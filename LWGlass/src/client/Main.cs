﻿using JimmysUnityUtilities;
using LogicAPI.Data;
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
            obj.GetComponent<MeshRenderer>().sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, 0.7f);
        }

        protected override void DataUpdate()
        {
            GameObject obj = Decorations[0].DecorationObject;
            obj.GetComponent<MeshRenderer>().sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, 0.2f);
            if (SizeX == previousSizeX && SizeZ == previousSizeZ)
                return;
            previousSizeX = SizeX;
            previousSizeZ = SizeZ;
            obj.transform.localScale = new Vector3(SizeX, .5f, SizeZ) * 0.3f;
        }
        protected override ChildPlacementInfo GenerateChildPlacementInfo()
        {
            ChildPlacementInfo childPlacementInfo = new ChildPlacementInfo();
            childPlacementInfo.Points = new FixedPlacingPoint[SizeZ * SizeX];
            
            int i = 0;
            for (int iX = 0; iX < SizeX; iX++)
            {
                for (int iZ = 0; iZ < SizeZ; iZ++)
                {
                    childPlacementInfo.Points[i++] = new FixedPlacingPoint()
                    {
                        Position = new Vector3(iX, .5f, iZ)
                    };
                }
            }
            
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
            meshRenderer.sharedMaterial = LogicWorld.References.MaterialsCache.StandardUnlitColorTransparent(Color, 0.2f);
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