#if UNITY_EDITOR
using EasyFramework;

public static class F
{
    public static IResLoader ResLoader => EasyFramework.Editor.ResLoader.Instance;
}
#endif