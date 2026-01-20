namespace TransGuideApi.Services
{
    public class ResetCodeService
    {
        private readonly Dictionary<string, (string Code, DateTime Expiry)> _codes = new();

        public string GenerateCode(string email)
        {
            var code = new Random().Next(100000, 999999).ToString();
            _codes[email] = (code, DateTime.UtcNow.AddMinutes(15));
            return code;
        }

        public bool IsValid(string email, string code)
        {
            if (!_codes.TryGetValue(email, out var stored))
                return false;

            if (DateTime.UtcNow > stored.Expiry)
            {
                _codes.Remove(email);
                return false;
            }

            return stored.Code == code;
        }

        public void RemoveCode(string email)
        {
            _codes.Remove(email);
        }
    }
}