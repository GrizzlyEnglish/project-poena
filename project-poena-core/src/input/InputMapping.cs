namespace Project_Poena.Input
{
    public class InputMapping
    {
        public string raw_input { get; private set; }
        public string mapped_input { get; private set; }

        public InputMapping(string raw_input, string mapped_input)
        {
            this.raw_input = raw_input;
            this.mapped_input = mapped_input;
        }
    }

}