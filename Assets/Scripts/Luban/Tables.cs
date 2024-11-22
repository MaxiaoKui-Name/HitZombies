
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;

namespace cfg
{
public partial class Tables
{
    public Table_Global TableGlobal {get; }
    public Table_PlayerConfig TablePlayerConfig {get; }
    public Table_levelConfig TableLevelConfig {get; }
    public Table_danConfig TableDanConfig {get; }
    public Table_Doorgenerate TableDoorgenerate {get; }
    public Table_Doorcontent TableDoorcontent {get; }
    public Table_monsterConfig TableMonsterConfig {get; }
    public Table_Boxgenerate TableBoxgenerate {get; }
    public Table_boxcontent TableBoxcontent {get; }
    public Table_transmitConfig TableTransmitConfig {get; }
    public Table_DailyConfig TableDailyConfig {get; }
    public Table_turntableConfig TableTurntableConfig {get; }
    public Table_JumpConfig TableJumpConfig {get; }
    public Table_beginnerConfig TableBeginnerConfig {get; }
    public Table_settlementConfig TableSettlementConfig {get; }
    public Table_soundConfig TableSoundConfig {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        TableGlobal = new Table_Global(loader("table_global"));
        TablePlayerConfig = new Table_PlayerConfig(loader("table_playerconfig"));
        TableLevelConfig = new Table_levelConfig(loader("table_levelconfig"));
        TableDanConfig = new Table_danConfig(loader("table_danconfig"));
        TableDoorgenerate = new Table_Doorgenerate(loader("table_doorgenerate"));
        TableDoorcontent = new Table_Doorcontent(loader("table_doorcontent"));
        TableMonsterConfig = new Table_monsterConfig(loader("table_monsterconfig"));
        TableBoxgenerate = new Table_Boxgenerate(loader("table_boxgenerate"));
        TableBoxcontent = new Table_boxcontent(loader("table_boxcontent"));
        TableTransmitConfig = new Table_transmitConfig(loader("table_transmitconfig"));
        TableDailyConfig = new Table_DailyConfig(loader("table_dailyconfig"));
        TableTurntableConfig = new Table_turntableConfig(loader("table_turntableconfig"));
        TableJumpConfig = new Table_JumpConfig(loader("table_jumpconfig"));
        TableBeginnerConfig = new Table_beginnerConfig(loader("table_beginnerconfig"));
        TableSettlementConfig = new Table_settlementConfig(loader("table_settlementconfig"));
        TableSoundConfig = new Table_soundConfig(loader("table_soundconfig"));
        ResolveRef();
    }
    
    private void ResolveRef()
    {
        TableGlobal.ResolveRef(this);
        TablePlayerConfig.ResolveRef(this);
        TableLevelConfig.ResolveRef(this);
        TableDanConfig.ResolveRef(this);
        TableDoorgenerate.ResolveRef(this);
        TableDoorcontent.ResolveRef(this);
        TableMonsterConfig.ResolveRef(this);
        TableBoxgenerate.ResolveRef(this);
        TableBoxcontent.ResolveRef(this);
        TableTransmitConfig.ResolveRef(this);
        TableDailyConfig.ResolveRef(this);
        TableTurntableConfig.ResolveRef(this);
        TableJumpConfig.ResolveRef(this);
        TableBeginnerConfig.ResolveRef(this);
        TableSettlementConfig.ResolveRef(this);
        TableSoundConfig.ResolveRef(this);
    }
}

}
