using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ComponentsAndTags
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly TransformAspect _transform;
        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;

        public float3 Position
        {
            get => _transform.LocalPosition;
            set => _transform.LocalPosition = value;
        }

        public float RiseRate
        {
            get => _zombieRiseRate.ValueRO.Value;
        }

        public bool IsAboveGround => _transform.Position.y >= 0f;

        public void SetAtGroundLevel()
        {
            var position = _transform.Position;
            position.y = 0f;
            _transform.Position = position;
        }
    }
}