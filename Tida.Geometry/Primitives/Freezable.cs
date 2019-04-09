using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.Primitives {
    /// <summary>
    /// 可冻结单元,可指示是否禁止属性的更改;
    /// </summary>
    public abstract class Freezable {
        /// <summary>
        /// 是否能够被冻结;
        /// </summary>
        public bool IsFrozen { get; private set; }

        /// <summary>
        /// 冻结;
        /// </summary>
        public void Freeze() {
            IsFrozen = true;
            OnFreeze();
        }

        protected virtual void OnFreeze() {

        }

        /// <summary>
        /// 设定可冻结属性,如果处于冻结状态,将抛出一个不可操作异常;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="propertyName"></param>
        protected void SetFreezableProperty<T>(ref T member,T value,[CallerMemberName]string propertyName = null) {
            if (IsFrozen) {
                throw new InvalidOperationException($"Property {propertyName} can't be set,Because this object has already been frozen.");
            }

            member = value;
        }

        
    }
}
