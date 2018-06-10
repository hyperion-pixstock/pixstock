using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hyperion.Pf.Entity
{
    public interface IEavType<T>
    {

        #region Public Properties

        T Value { get; set; }

        #endregion Public Properties
    }

    public abstract class Eav_Attribute<T> : IEntity<T> {


        
        #region Private Fields

        private string _AttributeCode;

        #endregion Private Fields


        #region Public Properties

        public virtual T Id { get; set; }

        public string AttributeCode
        {
            get
            { return _AttributeCode; }
            set
            {
                if (_AttributeCode == value)
                    return;
                _AttributeCode = value;
            }
        }

        #endregion Public Properties

    }

    /// <summary>
    /// EAVデータを格納するコレクション
    /// </summary>
    /// <typeparam name="T">EAVデータの型</typeparam>
    /// <typeparam name="EAVT">EAVデータORMクラス</typeparam>
    /// <typeparam name="EAVT2">EAVマスタデータORMクラス</typeparam>
    public abstract class EavAttributeVarcharListBase<T, EAVT, EAVT2, KEY> : IEnumerable<T>
        where EAVT : IEavType<T>, new()
        where EAVT2 : Eav_Attribute<KEY>
    {

        #region Protected Fields

        protected readonly string _AttributeCode;

        protected IList<EAVT2> _Attributes;

        #endregion Protected Fields


        #region Public Constructors

        public EavAttributeVarcharListBase(IList<EAVT2> Attributes, string AttributeCode)
        {
            _Attributes = Attributes;
            _AttributeCode = AttributeCode;
        }

        #endregion Public Constructors


        #region Public Properties

        public int Count
        {
            get
            {
                var source = GetEnumerator();
                ICollection c = source as ICollection;
                if (c != null)
                    return c.Count;

                int result = 0;

                while (source.MoveNext())
                    result++;

                return result;
            }
        }

        #endregion Public Properties


        #region Public Indexers

        public T this[int key]
        {
            get
            {
                var b = true;
                var r = this.GetEnumerator();
                r.Reset();
                while (key < -1 && b)
                {
                    key--;
                    b = r.MoveNext();
                }

                if (b)
                    return r.Current;

                return default(T);
            }

            set
            {
                dynamic r = Get(key);
                if (r != null) r.Value = value;
            }
        }

        #endregion Public Indexers


        #region Protected Methods

        protected void Add(EAVT2 item)
        {
            this._Attributes.Add(item);
        }

        protected Eav_Attribute<KEY> Get(int index)
        {
            var r = this._Attributes.Where(p => p.AttributeCode == _AttributeCode);
            return r.AsQueryable().ElementAt(index);
        }

        #endregion Protected Methods


        #region Public Methods

        public void Add(T item)
        {
            var myItem = new EAVT();
            myItem.Value = item;

            var a = myItem as EAVT2;
            a.AttributeCode = _AttributeCode;


            this._Attributes.Add(a);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._Attributes.Where(p => p.AttributeCode == _AttributeCode)
                    .Cast<EAVT>()
                    .Select((p, a) => p.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Public Methods
    }
}