using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.ComponentsAndTags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.ECS.Authorings
{
    public readonly partial struct CharacterMoveAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly TransformAspect _transform;
        private readonly RefRW<CharacterTimer> _walkTimer;
        private readonly RefRO<CharacterMoveProperties> _walkProperties;
        private readonly RefRO<CharacterHeading> _heading;

        private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
        private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;
        private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
        private float Heading => _heading.ValueRO.Value;

        private float WalkTimer
        {
            get => _walkTimer.ValueRO.Value;
            set => _walkTimer.ValueRW.Value = value;
        }

        public void Walk(float deltaTime)
        {
            WalkTimer += deltaTime;
            _transform.Position += _transform.Forward * WalkSpeed * deltaTime;

            var swayAngle = WalkAmplitude * math.sin(WalkFrequency * WalkTimer);
            _transform.Rotation = quaternion.Euler(0, Heading, swayAngle);


            //var pos = _transform.LocalPosition;
            //var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
            //_transform.Rotation = quaternion.Euler(0, angle, swayAngle);
            ////_transform.LocalRotation = quaternion.RotateY(angle);
        }

        public void Walk2(float deltaTime)
        {
            // Notice that this is a lambda being passed as parameter to ForEach.
            // 이것은 ForEach에 매개변수로 전달되는 람다임을 알 수 있습니다.
            var pos = _transform.LocalPosition;

            // This does not modify the actual position of the tank, only the point at which we sample the 3D noise function.
            // This way, every tank is using a different slice and will move along its own different random flow field.

            // 이렇게 하면 탱크의 실제 위치는 수정되지 않고 3D 노이즈 함수를 샘플링하는 지점만 수정됩니다. 
            // 이렇게 하면 모든 탱크가 다른 슬라이스를 사용하며 각기 다른 랜덤 플로우 필드를 따라 이동합니다.
            var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;

            var dir = float3.zero;
            math.sincos(angle, out dir.x, out dir.z);
            _transform.LocalPosition += dir * deltaTime * 5.0f;
            _transform.LocalRotation = quaternion.RotateY(angle);
        }
    }
}
