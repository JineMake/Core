using System;
using System.Collections.Generic;
using System.Text;

    /// <summary>
    /// 泛型单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
  public  class Singleton<T> where T: class,new()
    {
        private static T t = default(T);
        /// <summary>
        /// 获得泛型单例
        /// </summary>
        public static T GetInstance
        {
            get
            {
                if (t==null)
                {
                    t = new T();
                }
                return t;
            }
        }
    }
