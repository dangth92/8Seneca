namespace EightSeneca.Wpf.Contracts
{
    public interface IRegistryService
    {
        // Lấy giá trị string từ registry
        string GetString(string keyName);

        // Lấy giá trị int từ registry
        int GetInt(string keyName);

        // Set giá trị string
        void SetString(string keyName, string value);

        // Set giá trị int
        void SetInt(string keyName, int value);

        // Reset key về default value
        void Reset(string keyName);

        // Tạo group và các key mặc định
        void CreateDefaults();

        // Xóa toàn bộ group + key
        void RemoveAll();
    }
}
