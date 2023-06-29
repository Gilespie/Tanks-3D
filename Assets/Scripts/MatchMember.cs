using Mirror;
using UnityEngine.Events;

[System.Serializable]
public class MatchMemberData
{
    public int Id;
    public string Nickname;
    public int TeamId;
    public NetworkIdentity Member;

    public MatchMemberData(int id, string nickname, int teamId, NetworkIdentity member)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
        Member = member;
    }
}

public static class MatchMemberExtention
{
    public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
        writer.WriteNetworkIdentity(value.Member);
    }

    public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
    {
        return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
    }
}

public class MatchMember : NetworkBehaviour
{
    public static event UnityAction<MatchMember, int> ChangedFrag;

    public Vehicle ActiveVehicle { get; set; }

    #region DATA

    protected MatchMemberData data;
    public MatchMemberData Data => data;

    [Command]
    protected void CmdUpdatePlayer(MatchMemberData data)
    {
        this.data = data;
    }

    #endregion

    #region FRAGS

    [SyncVar(hook = nameof(OnChangedFrag))]
    protected int fragAmount;

    [Server]
    public void SvAddFrags()
    {
        fragAmount++;

        ChangedFrag?.Invoke(this, fragAmount);
    }

    [Server]
    public void SvResetFrags()
    {
        fragAmount = 0;
    }

    private void OnChangedFrag(int old, int newValue)
    {
        ChangedFrag?.Invoke(this, newValue);
    }

    #endregion

    #region NICKNAME

    [SyncVar(hook = nameof(OnNicknameChanged))] protected string m_Nickname;
    public string Nickname => m_Nickname;

    [Command]
    protected void CmdSetName(string name)
    {
        m_Nickname = name;
        gameObject.name = name;
    }

    private void OnNicknameChanged(string old, string newValue)
    {
        gameObject.name = newValue;
    }

    #endregion

    #region TEAMID

    [SyncVar] protected int m_TeamId;
    public int TeamId => m_TeamId;

    #endregion
}