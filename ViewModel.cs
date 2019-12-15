namespace Multitasking_ThreadPool
{
    class ViewModel
    {
        public ViewModel(string v1, string v2)
        {
            this.inputPath = v1;
            this.outputPath = v2;
        }

        public string inputPath { get; set; }
        public string outputPath { get; set; }


    }
}