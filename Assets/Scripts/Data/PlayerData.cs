public class PlayerData : BaseData
{
    public event EventDataHandler<PlayerData> RefreshEvent;

    public string CharacterID { get; private set; }

    public PlayerData() {
        CharacterID = "C0000";
    }

    ~PlayerData() {

    }

    public void SetCharacterID(string id) {
        CharacterID = id;
    }

    void Update(PlayerData data) {
        RefreshEvent?.Invoke(data);
    }

}
