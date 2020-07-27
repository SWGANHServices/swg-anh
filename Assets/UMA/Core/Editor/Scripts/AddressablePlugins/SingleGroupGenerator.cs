﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMA;
using UnityEditor;
using UnityEngine;
using UMA.CharacterSystem;

#if UMA_ADDRESSABLES
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;


namespace UMA
{
    public class SingleGroupGenerator : IUMAAddressablePlugin
    {
        public UMAAssetIndexer Index;
        List<UMAPackedRecipeBase> Recipes;
        Dictionary<AssetItem, List<string>> AddressableItems = new Dictionary<AssetItem, List<string>>();
        Dictionary<string, List<string>> RecipeExtraLabels = new Dictionary<string, List<string>>();

        const string SharedGroupName = "UMA_SharedItems";

        public string Menu
        {
            get
            {
                return "Generate Single group (fast)";
            }
        }

        public void Complete()
        {
            try
            {
                bool IncludeRecipes = UMAEditorUtilities.GetConfigValue(UMAEditorUtilities.ConfigToggle_IncludeRecipes, false);
                bool IncludeOthers = UMAEditorUtilities.GetConfigValue(UMAEditorUtilities.ConfigToggle_IncludeOther, false);
                string DefaultAddressableLabel = UMAEditorUtilities.GetDefaultAddressableLabel();

                RecipeExtraLabels = new Dictionary<string, List<string>>();
                
                if (UMAEditorUtilities.GetConfigValue(UMAEditorUtilities.ConfigToggle_AddCollectionLabels, false))
                {
                    var WardrobeCollections = UMAAssetIndexer.Instance.GetAllAssets<UMAWardrobeCollection>();
                    foreach (var wc in WardrobeCollections)
                    {
                        if (wc == null) continue;
                        string label = wc.AssignedLabel;
                        List<string> recipes = wc.wardrobeCollection.GetAllRecipeNamesInCollection();
                        foreach (string recipe in recipes)
                        {
                            if (RecipeExtraLabels.ContainsKey(recipe) == false)
                            {
                                RecipeExtraLabels.Add(recipe, new List<string>());
                            }
                            RecipeExtraLabels[recipe].Add(label);
                        }
                    }
                }

                float pos = 0.0f;
                float inc = 1.0f / Recipes.Count;
                foreach (UMAPackedRecipeBase uwr in Recipes)
                {
                    List<string> ExtraLabels = new List<string>();

                    if (RecipeExtraLabels.ContainsKey(uwr.name))
                    {
                        ExtraLabels = RecipeExtraLabels[uwr.name];
                    }

                    EditorUtility.DisplayProgressBar("Generating", "processing recipe: " + uwr.name, pos);
                    List<AssetItem> items = Index.GetAssetItems(uwr, true);
                    foreach (AssetItem ai in items)
                    {
                        if (AddressableItems.ContainsKey(ai) == false)
                        {
                            AddressableItems.Add(ai, new List<string>());
                            AddressableItems[ai].Add(DefaultAddressableLabel);
                        }
                        AddressableItems[ai].Add(uwr.AssignedLabel);
                        AddressableItems[ai].AddRange(ExtraLabels);
                    }
                    if (IncludeRecipes)
                    {
                        AssetItem RecipeItem = UMAAssetIndexer.Instance.GetRecipeItem(uwr);
                        if (AddressableItems.ContainsKey(RecipeItem) == false)
                        {
                            AddressableItems.Add(RecipeItem, new List<string>());
                            AddressableItems[RecipeItem].Add(DefaultAddressableLabel);
                        }
                        AddressableItems[RecipeItem].Add(uwr.AssignedLabel);
                        AddressableItems[RecipeItem].Add("UMA_Recipes");
                        AddressableItems[RecipeItem].AddRange(ExtraLabels);
                    }
                    pos += inc;
                }


                if (IncludeOthers)
                {
                    AddAssetItems(typeof(RaceData), DefaultAddressableLabel);
                    AddAssetItems(typeof(RuntimeAnimatorController), DefaultAddressableLabel);
                    AddAssetItems(typeof(TextAsset), DefaultAddressableLabel);
                    AddAssetItems(typeof(DynamicUMADnaAsset), DefaultAddressableLabel);
                }


                // Create the shared group that has each item packed separately.
                AddressableAssetGroup sharedGroup = Index.AddressableSettings.FindGroup(SharedGroupName);
                if (sharedGroup == null)
                {
                    sharedGroup = Index.AddressableSettings.CreateGroup(SharedGroupName, false, false, true, Index.AddressableSettings.DefaultGroup.Schemas);
                    sharedGroup.GetSchema<BundledAssetGroupSchema>().BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                }

                pos = 0.0f;
                inc = 1.0f / AddressableItems.Count;

                StringBuilder sb = new StringBuilder();
                foreach (AssetItem ai in AddressableItems.Keys)
                {
                    ai.IsAddressable = true;
                    ai.AddressableAddress = ""; // let the system assign it if we are generating.
                    ai.AddressableGroup = sharedGroup.name;
                    EditorUtility.DisplayProgressBar("Generating", "Processing Asset: " + ai.Item.name, pos);

                    sb.Clear();
                    foreach (string s in AddressableItems[ai])
                    {
                        sb.Append(s);
                        sb.Append(';');
                    }
                    ai.AddressableLabels = sb.ToString();

                    bool found = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(ai.Item.GetInstanceID(), out string itemGUID, out long localID);

                    Index.AddItemToSharedGroup(itemGUID, ai.AddressableAddress, AddressableItems[ai], sharedGroup);
                    if (ai._Type == typeof(OverlayDataAsset))
                    {
                        OverlayDataAsset od = ai.Item as OverlayDataAsset;
                        if (od == null)
                        {
                            Debug.Log("Invalid overlay in recipe: " + ai._Name + ". Skipping.");
                            continue;
                        }
                        foreach (Texture tex in od.textureList)
                        {
                            if (tex == null) continue;
                            if (tex as Texture2D == null)
                            {
                                Debug.Log("Texture is not Texture2D!!! " + tex.name);
                                continue;
                            }
                            string Address = "Texture2D-" + tex.name + "-" + tex.GetInstanceID();

                            found = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tex.GetInstanceID(), out string texGUID, out long texlocalID);
                            if (found)
                            {
                                Index.AddItemToSharedGroup(texGUID, AssetItem.AddressableFolder + Address, AddressableItems[ai], sharedGroup);
                            }
                        }
                    }
                    pos += inc;
                }

                Index.AssignAddressableInformation();

                Type[] types = Index.GetTypes();

                foreach (Type t in types)
                {
                    Index.ReleaseReferences(t);
                }

                Index.CleanupAddressables(true);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                UMAAssetIndexer.Instance.ForceSave();
            }
        }

        public bool Prepare()
        {
            Index = UMAAssetIndexer.Instance;

            foreach (Type t in Index.GetTypes())
            {
                Index.ClearAddressableFlags(t);
            }

            Recipes = new List<UMAPackedRecipeBase>();
            return true;
        }

        public List<string> ProcessItem(AssetItem ai)
        {
            // This generator does not process single items.
            return null;
        }

        /// <summary>
        /// Process the recipes. This generator simply accumulates the recipes for processing
        /// once all recipes are in the list.
        /// </summary>
        /// <param name="recipe"></param>
        public void ProcessRecipe(UMAPackedRecipeBase recipe)
        {
            Recipes.Add(recipe);
        }

        private void AddAssetItems(Type t, string DefaultLabel)
        {
            List<AssetItem> Items = UMAAssetIndexer.Instance.GetAssetItems(t);
            foreach (AssetItem item in Items)
            {
                AddressableItems.Add(item, new List<string>());
                AddressableItems[item].Add(DefaultLabel);
            }
        }
    }
}
#endif