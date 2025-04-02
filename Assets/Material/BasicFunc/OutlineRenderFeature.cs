using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class OutlineRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    // 定义3个共有变量
    public class OutlineSettings
    {
        public Shader outlineShader; // 设置后处理shader
        //public Material material; //后处理Material
       // public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; // 定义事件位置，放在了官方的后处理之前
    }
    public OutlineSettings settings = new OutlineSettings();
    


    public class OutlinePass : ScriptableRenderPass
    {
        static readonly string renderTag = "L3_OutlinePass"; // 定义渲染Tag
        ProfilingSampler m_ProfilingSampler = new(renderTag);

        OutlineSettings settings;
        Material outlineMaterial;
        OutlineVolume outlineVolume;  // 传递到volume,OutlineVolume是Volume那个类定义的类名

        private RTHandle source;
        private RTHandle temporaryTexture1;

        public OutlinePass(OutlineSettings outlineSettings)
        {
            outlineMaterial = new Material(outlineSettings.outlineShader) { hideFlags = HideFlags.DontSave };
            settings = outlineSettings;
        }

        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            //初始化设置这张图
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.width = renderingData.cameraData.cameraTargetDescriptor.width;
            desc.height = renderingData.cameraData.cameraTargetDescriptor.height;

            //desc.width = renderingData.cameraData.scaledWidth;
            //desc.height = renderingData.cameraData.scaledHeight;

            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture1, desc, FilterMode.Bilinear);//分配
            cmd.SetGlobalTexture("_TemporaryTexture1", temporaryTexture1.nameID);//这样通过名字_MyColorTex就能在shader中找到_GrabTex
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 判断是否开启后处理
            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            //volume
            var stack = VolumeManager.instance.stack;             // 传入volume
            outlineVolume = stack.GetComponent<OutlineVolume>();  // 拿到我们的volume
            if (outlineVolume == null)
            {
                Debug.LogError("Volume组件获取失败");
                return;
            }

            //excute
            var cmd = CommandBufferPool.Get(renderTag);     // 设置渲染标签

            using (new UnityEngine.Rendering.ProfilingScope(cmd, m_ProfilingSampler))
            {                                                                      
                var camera = renderingData.cameraData.camera;                         // 传入摄像机
                Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true).inverse;

                outlineMaterial.SetColor("_Color", outlineVolume.OutlineColor.value);   // 获取value 组件的颜色

                outlineMaterial.SetMatrix("_ClipToView", clipToView);   // 反向输出到Shader

                outlineMaterial.SetFloat("_Scale", outlineVolume.Scale.value);
                outlineMaterial.SetFloat("_DepthThreshold", outlineVolume.DepthThreshold.value);
                outlineMaterial.SetFloat("_NormalThreshold", outlineVolume.NormalThreshold.value);

                outlineMaterial.SetFloat("_DepthNormalThreshold", outlineVolume.DepthNormalThreshold.value);
                outlineMaterial.SetFloat("_DepthNormalThresholdScale", outlineVolume.DepthNormalThresholdScale.value);


                cmd.Blit(source, temporaryTexture1);                            // 设置后处理
                cmd.Blit(temporaryTexture1, source, outlineMaterial, 0);
            }
            context.ExecuteCommandBuffer(cmd);              // 执行函数
            cmd.Clear();
            cmd.Dispose();
        }

        public void OnDispose()
        {
            temporaryTexture1?.Release();
        }

    }




    OutlinePass outlinePass;

    public override void Create()
    {
        //name = "OutlinePass"; // 外部显示的名字
        outlinePass = new OutlinePass(settings);
        outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // 使用 RTHandle 获取当前渲染目标
        RTHandle cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        outlinePass.Setup(cameraColorHandle);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        outlinePass.OnDispose();
    }

}

