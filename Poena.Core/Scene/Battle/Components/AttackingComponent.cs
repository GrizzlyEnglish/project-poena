using Poena.Core.Common.Enums;
using Poena.Core.Entity.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Scene.Battle.Components
{
    public class AttackingComponent : Component
    {
        public AttackType AttackType { get; set; }

        public override void Initialize()
        {
        }
    }
}
