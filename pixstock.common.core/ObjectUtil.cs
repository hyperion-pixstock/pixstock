using System;

namespace Katalib.Nc.Standard
{
    public static class ObjectUtil
    {
        /// <summary>
        /// 対象がジェネリクスオブジェクト、またはジェネリクスクラスのサブクラスかを検証します
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <see href="https://stackoverflow.com/questions/982487/testing-if-object-is-of-generic-type-in-c-sharp" />
        public static bool IsInstanceOfGenericType(Type genericType, object instance)
        {
            Type type = instance.GetType();
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}