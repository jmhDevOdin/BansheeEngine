//********************************** Banshee Engine (www.banshee3d.com) **************************************************//
//**************** Copyright (c) 2016 Jonathan Harrison (harrison.j@banshee3d.com). All rights reserved. **********************//
using System.Collections;
using System.Collections.Generic;
using bs;

namespace bs.Editor
{
    /** @addtogroup Inspectors
     *  @{
     */

    /// <summary>
    /// Renders an inspector for the <see cref="Bone"/> component.
    /// </summary>
    [CustomInspector(typeof(Bone))]
    internal class BoneInspector : Inspector
    {
        private GUIListBoxField boneField;
        private InspectableState modifyState;

        private string selectedBoneName;

        /// <inheritdoc/>
        protected internal override void Initialize()
        {
            Layout.Clear();

            Bone bone = InspectedObject as Bone;
            if (bone == null)
                return;

            string[] boneNames = GetBoneNames(bone);
            if(boneNames == null)
                boneNames = new string[0];

            boneField = new GUIListBoxField(boneNames, false, new LocEdString("Bone"));

            Layout.AddElement(boneField);

            boneField.OnSelectionChanged += x =>
            {
                selectedBoneName = boneNames[x];

                StartUndo("bone");
                bone.Name = selectedBoneName;
                EndUndo();
                
                MarkAsModified();
                ConfirmModify();
            };
        }

        /// <inheritdoc/>
        protected internal override InspectableState Refresh(bool force = false)
        {
            Bone bone = InspectedObject as Bone;
            if (bone == null)
                return InspectableState.NotModified;

            if (selectedBoneName != bone.Name)
            {
                string[] boneNames = GetBoneNames(bone);

                if (boneNames != null)
                {
                    for (int i = 0; i < boneNames.Length; i++)
                    {
                        if (bone.Name == boneNames[i])
                        {
                            selectedBoneName = bone.Name;
                            boneField.Index = i;
                            break;
                        }
                    }
                }
            }

            InspectableState oldState = modifyState;
            if (modifyState.HasFlag(InspectableState.Modified))
                modifyState = InspectableState.NotModified;

            return oldState;
        }

        /// <summary>
        /// Finds all available bones for the animation the provided bone is a part of.
        /// </summary>
        /// <param name="bone">Bone for which to return the parent skeleton's bones.</param>
        /// <returns>List of bones if parent skeleton is found, or null.</returns>
        private string[] GetBoneNames(Bone bone)
        {
            Animation animParent = null;

            SceneObject currentSO = bone.SceneObject;
            while (currentSO != null)
            {
                animParent = currentSO.GetComponent<Animation>();
                if (animParent != null)
                    break;

                currentSO = currentSO.Parent;
            }

            if(animParent == null)
                return null;

            Renderable renderable = animParent.SceneObject.GetComponent<Renderable>();
            if (renderable == null)
                return null;

            RRef<Mesh> mesh = renderable.Mesh;
            if (mesh == null)
                return null;

            Skeleton skeleton = mesh.Value.Skeleton;
            if (skeleton == null)
                return null;

            string[] boneNames = new string[skeleton.NumBones];
            for (int i = 0; i < boneNames.Length; i++)
                boneNames[i] = skeleton.GetBoneInfo(i).name;

            return boneNames;
        }
        
        /// <summary>
        /// Marks the contents of the inspector as modified.
        /// </summary>
        protected void MarkAsModified()
        {
            modifyState |= InspectableState.ModifyInProgress;
        }

        /// <summary>
        /// Confirms any queued modifications.
        /// </summary>
        protected void ConfirmModify()
        {
            if (modifyState.HasFlag(InspectableState.ModifyInProgress))
                modifyState |= InspectableState.Modified;
        }
    }

    /** @} */
}