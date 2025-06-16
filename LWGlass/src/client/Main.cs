using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicSettings;
using LogicWorld.Building.Overhaul;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Resizing;
using LogicWorld.Interfaces;
using LogicWorld.Interfaces.Building;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using Unified.UniversalBlur.Runtime;
using UnityEngine;

namespace LWGlass.Client
{
    public class Glass : 
        ComponentClientCode<CircuitBoard.IData>,
        IColorableClientCode,
        IResizableX,
        IResizableZ
    {
        [Setting_SliderFloat("LWGlass.Glass.GlassTransparency")]
        public static float GlassTransparency {
            get => _glassTransparency;
            set
            {
                _glassTransparency = value;
                updateGlassTransparency();
            }
        }
        private static float _glassTransparency = 15;

        protected static void updateGlassTransparency()
        {
            var world = Instances.MainWorld;
            if (world == null)
            {
                return;
            }
            var glass = world.ComponentType("LWGlass.Client.Glass");
            foreach(var kvp in world.Data.AllComponents)
            {
                var (addr, data) = kvp;
                if (data.Data.Type == glass)
                {
                    GameObject obj = Decorations[0].DecorationObject;
                    obj.GetComponent<MeshRenderer>().sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, (float) _glassTransparency/10);
                }
            }
        }
        
        private int previousSizeX;
        private int previousSizeZ;

        public Color24 Color
        {
            get => Data.Color;
            set => Data.Color = value;
        }
        public string ColorsFileKey => "Boards";
        public float MinColorValue => 0;

        public int SizeX
        {
            get => Data.SizeX;
            set => Data.SizeX = value;
        }
        public int MinX => 1;
        public int MaxX => 80;
        public float GridIntervalX => 1f;

        public int SizeZ
        {
            get => Data.SizeZ;
            set => Data.SizeZ = value;
        }
        public int MinZ => 1;
        public int MaxZ => 80;
        public float GridIntervalZ => 1f; 
        
        protected override void OnComponentImaged()
        {
            Data.Color = new Color24(255, 255, 255);
            GameObject obj = Decorations[0].DecorationObject;
            obj.GetComponent<MeshRenderer>().sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, (float) _glassTransparency/10);
        }

        protected override void DataUpdate()
        {
            GameObject obj = Decorations[0].DecorationObject;
            obj.GetComponent<MeshRenderer>().sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, (float) _glassTransparency/10);
            if (SizeX == previousSizeX && SizeZ == previousSizeZ)
                return;
            previousSizeX = SizeX;
            previousSizeZ = SizeZ;
            obj.transform.localScale = new Vector3(SizeX, .5f, SizeZ) * 0.3f;
        }
        protected override ChildPlacementInfo GenerateChildPlacementInfo()
        {
            ChildPlacementInfo childPlacementInfo = new ChildPlacementInfo();
            childPlacementInfo.Points = new FixedPlacingPoint[SizeZ * SizeX * 2];
            
            int i = 0;
            for (int iX = 0; iX < SizeX; iX++)
            {
                for (int iZ = 0; iZ < SizeZ; iZ++)
                {
                    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
                    {
                        Position = new Vector3(iX, .5f, iZ),
                        UpDirection = new Vector3(0, 1, 0)
                    };
                    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
                    {
                        Position = new Vector3(iX, 0, iZ),
                        UpDirection = new Vector3(0, -1, 0)
                    };
                }
            }
            //for (int SX1 = 0; SX1 < SizeX; SX1++)
            //{
            //    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
            //    {
            //        Position = new Vector3(SX1, 0, -.3f ),
            //        UpDirection = new Vector3(0, 0, -1)
            //    };
            //    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
            //    {
            //        Position = new Vector3(SX1, 0, SizeZ+.3f ),
            //        UpDirection = new Vector3(0, 0, 1)
            //    };
            //}
            //for (int SX1 = 0; SX1 < SizeZ; SX1++)
            //{
            //    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
            //    {
            //        Position = new Vector3(-.3f, 0, SX1 ),
            //        UpDirection = new Vector3(-1, 0, 0)
            //    };
            //    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
            //    {
            //        Position = new Vector3(SizeX+.3f, 0, SX1 ),
            //        UpDirection = new Vector3(1, 0, 0)
            //    };
            //}
            return childPlacementInfo;
        }
        protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
        {
            var myGameObject = new GameObject("glass");
            myGameObject.transform.SetParent(parentToCreateDecorationsUnder);
            var collider = myGameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(.5f,.5f,.5f);
            var meshFilter = myGameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = LogicWorld.References.Meshes.OriginCube;
            var meshRenderer = myGameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, (float) _glassTransparency/10);
            return new IDecoration[]
            {
                new Decoration()
                {
                    DecorationObject = myGameObject,
                    LocalPosition = new Vector3(-.15f, 0, -.15f),
                    AutoSetupColliders = true,
                }
            };
        }

        protected override void SetDataDefaultValues()
        {
            Data.SizeX = 1;
            Data.SizeZ = 1;
            Data.Color = new Color24(120, 120, 120);
        }
    }
}
