//********************************** Banshee Engine (www.banshee3d.com) **************************************************//
//**************** Copyright (c) 2016 Jonathan Harrison (harrison.j@banshee3d.com). All rights reserved. **********************//
using System.Collections.Generic;
using bs;

namespace bs.Editor
{
    /** @addtogroup Inspectors
     *  @{
     */

    /// <summary>
    /// Renders an inspector for the <see cref="Shader"/> resource.
    /// </summary>
    [CustomInspector(typeof(Shader))]
    internal class ShaderInspector : Inspector
    {
        /// <inheritdoc/>
        protected internal override void Initialize()
        {
            // No GUI for shader resource
        }

        /// <inheritdoc/>
        protected internal override InspectableState Refresh(bool force = false)
        {
            return InspectableState.NotModified;
        }
    }

    /** @} */
}