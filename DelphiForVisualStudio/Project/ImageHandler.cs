/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Package
{
    public class ImageHandler
    {
        private ImageList imageList;
        private List<IntPtr> iconHandles;

        /// <summary>
        /// Builds an empty ImageHandler object.
        /// </summary>
        public ImageHandler()
        {
            this.imageList = null;
            this.iconHandles = null;
        }

        /// <summary>
        /// Builds an ImageHandler object from a Stream providing the bitmap that
        /// stores the images for the image list.
        /// </summary>
        public ImageHandler(Stream resourceStream)
        {
            if (null == resourceStream)
            {
                throw new ArgumentNullException("resourceStream");
            }
            imageList = Utilities.GetImageList(resourceStream);
        }

        /// <summary>
        /// Builds an ImageHandler object from an ImageList object.
        /// </summary>
        public ImageHandler(ImageList list)
        {
            if (null == list)
            {
                throw new ArgumentNullException("imageList");
            }
            imageList = list;
        }

        /// <summary>
        /// Closes the ImageHandler object freeing its resources.
        /// </summary>
        public void Close()
        {
            if (null != iconHandles)
            {
                foreach (IntPtr hnd in iconHandles)
                {
                    if (hnd != IntPtr.Zero)
                    {
                        NativeMethods.DestroyIcon(hnd);
                    }
                }
                iconHandles = null;
            }

            if (null != imageList)
            {
                imageList.Dispose();
                imageList = null;
            }
        }

        /// <summary>
        /// Add an image to the ImageHandler.
        /// </summary>
        public void AddImage(Image img)
        {
            if (null == img)
            {
                throw new ArgumentNullException("img");
            }
            if (null == imageList)
            {
                imageList = new ImageList();
            }
            imageList.Images.Add(img);
            if (null != iconHandles)
            {
                iconHandles.Add(IntPtr.Zero);
            }
        }

        /// <summary>
        /// Get or set the ImageList object for this ImageHandler.
        /// </summary>
        public ImageList ImageList
        {
            get { return imageList; }
            set 
            {
                Close();
                imageList = value;
            }
        }

        /// <summary>
        /// Returns the handle to an icon build from the image of index
        /// iconIndex in the image list.
        /// </summary>
        public IntPtr GetIconHandle(int iconIndex)
        {
            // Verify that the object is in a consistent state.
            if ((null == imageList))
            {
                throw new InvalidOperationException();
            }
            // Make sure that the list of handles is initialized.
            if (null == iconHandles)
            {
                InitHandlesList();
            }

            // Verify that the index is inside the expected range.
            if ((iconIndex < 0) || (iconIndex >= iconHandles.Count))
            {
                throw new ArgumentOutOfRangeException("iconIndex");
            }

            // Check if the icon is in the cache.
            if (IntPtr.Zero == iconHandles[iconIndex])
            {
                Bitmap bitmap = imageList.Images[iconIndex] as Bitmap;
                // If the image is not a bitmap, then we can not build the icon,
                // so we have to return a null handle.
                if (null == bitmap)
                {
                    return IntPtr.Zero;
                }

                iconHandles[iconIndex] = bitmap.GetHicon();
            }

            return iconHandles[iconIndex];
        }

        private void InitHandlesList()
        {
            iconHandles = new List<IntPtr>(imageList.Images.Count);
            for (int i = 0; i < imageList.Images.Count; ++i)
            {
                iconHandles.Add(IntPtr.Zero);
            }
        }
    }
}
