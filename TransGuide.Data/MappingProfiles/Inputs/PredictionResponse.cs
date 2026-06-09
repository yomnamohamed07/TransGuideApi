namespace TransGuide.Data.MappingProfiles.Inputs
{
    public class PredictionResponse
    {
        public string prediction { get; set; } = "";
        public string label { get; set; } = "";
        public float confidence { get; set; }
        public string status { get; set; } = "";
        public string current_word { get; set; } = "";

        public NLPResult? nlp_result { get; set; }
    }

    public class NLPResult
    {
        public string destination { get; set; } = "";
        public string status { get; set; } = "";
    }
}