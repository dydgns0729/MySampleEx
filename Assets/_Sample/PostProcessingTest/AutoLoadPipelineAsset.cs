using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MySampleEx
{
    [ExecuteAlways]
    public class AutoLoadPipelineAsset : MonoBehaviour
    {
        #region Variables
        public UniversalRenderPipelineAsset pipelineAsset;
        #endregion

        private void OnEnable()
        {
            if (pipelineAsset != null)
            {
                //GraphicsSettings.defaultRenderPipeline = pipelineAsset;
                GraphicsSettings.defaultRenderPipeline = pipelineAsset;
                QualitySettings.renderPipeline = pipelineAsset;
            }
        }

    }
}
