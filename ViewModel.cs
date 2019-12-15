namespace Multitasking_ThreadPool
{
    internal class ViewModel
    {
        public ViewModel(string inp, string outp)
        {
            InputPath = inp;
            OutputPath = outp;
        }

        public string InputPath { get; set; }
        public string OutputPath { get; set; }
    }
}