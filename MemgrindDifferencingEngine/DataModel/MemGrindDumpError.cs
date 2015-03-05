
namespace MemgrindDifferencingEngine.DataModel
{
    /// <summary>
    /// Some simple info for a generic memgrind stack dump error
    /// </summary>
    public class MemGrindDumpError
    {
        public int Occurances { get; set; }

        public MemGrindDumpError()
        {
            Occurances = 0;
        }

        public string Name { get; set; }
    }
}
