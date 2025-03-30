using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class OutlineRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    // ����3�����б���
    public class OutlineSettings
    {
        public Shader outlineShader; // ���ú���shader
        //public Material material; //����Material
       // public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; // �����¼�λ�ã������˹ٷ��ĺ���֮ǰ
    }
    public OutlineSettings settings = new OutlineSettings();
    


    public class OutlinePass : ScriptableRenderPass
    {
        static readonly string renderTag = "L3_OutlinePass"; // ������ȾTag
        ProfilingSampler m_ProfilingSampler = new(renderTag);

        OutlineSettings settings;
        Material outlineMaterial;
        OutlineVolume outlineVolume;  // ���ݵ�volume,OutlineVolume��Volume�Ǹ��ඨ�������

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
            //��ʼ����������ͼ
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.width = renderingData.cameraData.cameraTargetDescriptor.width;
            desc.height = renderingData.cameraData.cameraTargetDescriptor.height;

            //desc.width = renderingData.cameraData.scaledWidth;
            //desc.height = renderingData.cameraData.scaledHeight;

            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture1, desc, FilterMode.Bilinear);//����
            cmd.SetGlobalTexture("_TemporaryTexture1", temporaryTexture1.nameID);//����ͨ������_MyColorTex������shader���ҵ�_GrabTex
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // �ж��Ƿ�������
            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            //volume
            var stack = VolumeManager.instance.stack;             // ����volume
            outlineVolume = stack.GetComponent<OutlineVolume>();  // �õ����ǵ�volume
            if (outlineVolume == null)
            {
                Debug.LogError("Volume�����ȡʧ��");
                return;
            }

            //excute
            var cmd = CommandBufferPool.Get(renderTag);     // ������Ⱦ��ǩ

            using (new UnityEngine.Rendering.ProfilingScope(cmd, m_ProfilingSampler))
            {                                                                      
                var camera = renderingData.cameraData.camera;                         // ���������
                Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true).inverse;

                outlineMaterial.SetColor("_Color", outlineVolume.OutlineColor.value);   // ��ȡvalue �������ɫ

                outlineMaterial.SetMatrix("_ClipToView", clipToView);   // ���������Shader

                outlineMaterial.SetFloat("_Scale", outlineVolume.Scale.value);
                outlineMaterial.SetFloat("_DepthThreshold", outlineVolume.DepthThreshold.value);
                outlineMaterial.SetFloat("_NormalThreshold", outlineVolume.NormalThreshold.value);

                outlineMaterial.SetFloat("_DepthNormalThreshold", outlineVolume.DepthNormalThreshold.value);
                outlineMaterial.SetFloat("_DepthNormalThresholdScale", outlineVolume.DepthNormalThresholdScale.value);


                cmd.Blit(source, temporaryTexture1);                            // ���ú���
                cmd.Blit(temporaryTexture1, source, outlineMaterial, 0);
            }
            context.ExecuteCommandBuffer(cmd);              // ִ�к���
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
        //name = "OutlinePass"; // �ⲿ��ʾ������
        outlinePass = new OutlinePass(settings);
        outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // ʹ�� RTHandle ��ȡ��ǰ��ȾĿ��
        RTHandle cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        outlinePass.Setup(cameraColorHandle);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        outlinePass.OnDispose();
    }

}

