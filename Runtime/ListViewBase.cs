using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ZenListView
{
    public abstract class ListViewBase<TDataContext, TElement> : MonoBehaviour
        where TElement : ListElementBase<TDataContext, TElement>
    {
        [SerializeField] private Transform _parent;

        private readonly List<TElement> _elementsInUse = new List<TElement>();

        private IFactory<TElement> _factory;
        private IList<TDataContext> _data;

        public IList<TDataContext> Data
        {
            get => _data;
            set
            {
                _data = value; 
                Reload();
            }
        }

        [Inject]
        public void Init(ListElementBase<TDataContext, TElement>.Factory factory)
        {
            _factory = factory;
        }

        public void SetData(IList<TDataContext> data)
        {
            Data = data;
            Reload();
        }

        public void Reload()
        {
            while (_elementsInUse.Count < Data.Count)
            {
                TElement view = _factory.Create();
                view.transform.SetParent(_parent, false);
                _elementsInUse.Add(view);
            }

            while (_elementsInUse.Count > Data.Count)
            {
                int index = _elementsInUse.Count - 1;

                TElement v = _elementsInUse[index];
                if (v is IDisposable disposable)
                    disposable.Dispose();
                else
                    Destroy(v);

                _elementsInUse.RemoveAt(index);
            }

            for (int i = 0; i < _elementsInUse.Count; i++)
            {
                _elementsInUse[i].UpdateContent(Data[i]);
            }
        }

        private void OnDestroy()
        {
            if (!typeof(IDisposable).IsAssignableFrom(typeof(TElement))) return;

            foreach (TElement e in _elementsInUse)
            {
                if (e is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}