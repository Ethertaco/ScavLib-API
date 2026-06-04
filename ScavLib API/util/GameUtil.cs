using UnityEngine;

namespace ScavLib.util
{

    public static class GameUtil
    {

        public static bool IsInGame => PlayerCamera.main != null;

        public static bool IsWorldLoaded =>
            WorldGeneration.world != null && WorldGeneration.world.worldExists;

        public static WorldGeneration GetWorld()
        {
            return WorldGeneration.world;
        }

        public static Body GetBody()
        {
            return PlayerCamera.main != null ? PlayerCamera.main.body : null;
        }

        public static bool TryGetBody(out Body body)
        {
            body = GetBody();
            return body != null;
        }

        public static Vector2 GetPlayerPosition()
        {
            var body = GetBody();
            return body != null ? (Vector2)body.transform.position : Vector2.zero;
        }

        public static GameObject SpawnItem(string id, Vector2 position, float rotation = 0f)
        {
            if (!IsInGame)
            {
                ScavLibPlugin.Log.LogWarning($"[GameUtil] SpawnItem('{id}') called but no world is loaded.");
                return null;
            }

            var go = Utils.Create(id, position, rotation);
            if (go == null)
                ScavLibPlugin.Log.LogWarning($"[GameUtil] Failed to spawn '{id}' — resource not found.");

            return go;
        }

        public static GameObject SpawnItemAt(string id, Transform target)
        {
            if (target == null) return null;
            return SpawnItem(id, target.position, target.eulerAngles.z);
        }

        public static GameObject SpawnAtPlayer(string id)
        {
            if (!TryGetBody(out var body)) return null;

            var go = SpawnItem(id, body.transform.position);
            if (go == null) return null;

            if (go.TryGetComponent<Item>(out var item))
                body.AutoPickUpItem(item);

            return go;
        }

        public static void Log(string message)
        {
            if (ConsoleScript.instance == null) return;
            if (message == null) return;

            if (message.IndexOf('\n') < 0 && message.IndexOf('\r') < 0)
            {
                ConsoleScript.instance.ExecuteCommand("log " + message);
                return;
            }

            var lines = message.Split('\n');
            foreach (var rawLine in lines)
            {

                string line = rawLine.EndsWith("\r")
                    ? rawLine.Substring(0, rawLine.Length - 1)
                    : rawLine;

                if (line.Length == 0) continue;

                ConsoleScript.instance.ExecuteCommand("log " + line);
            }
        }

        public static void Alert(string text, bool important = false)
        {
            if (PlayerCamera.main == null) return;
            PlayerCamera.main.DoAlert(text, important);
        }

        public static void Notify(string text, bool important = false)
        {
            Alert(text, important);
            Log(text);
        }

        public static bool IsPointerOverUI()
        {
            return UIUtil.IsPointerOverUIElement();
        }
    }
}
