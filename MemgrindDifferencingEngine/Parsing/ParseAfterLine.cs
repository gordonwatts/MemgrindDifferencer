using System.Collections.Generic;

namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Process a bunch of prarse items, but only after a particular line has been seen.
    /// </summary>
    class ParseAfterLine : ParseItemBase, IEnumerable<ParseItemBase>
    {
        private List<ParseItemBase> _items = new List<ParseItemBase>();
        bool _active = false;
        private string _startsWith;

        public ParseAfterLine(string startsWith)
        {
            _startsWith = startsWith;
        }

        public void Add(ParseItemBase item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// If we've been activated, then process each line.
        /// </summary>
        /// <param name="line"></param>
        public override void Process(string line)
        {
            if (_active)
            {
                foreach (var item in _items)
                {
                    item.Process(line);
                }
            }
            else
            {
                _active = line.StartsWith(_startsWith);
            }
        }

        public IEnumerator<ParseItemBase> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
