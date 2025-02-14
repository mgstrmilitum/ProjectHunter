using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    public string destinationSceneName; // Changed from Object to string
    public string destSpawnName;

    void OnTriggerEnter(Collider collider)
    {
        AbletoTeleport abletotelport = collider.GetComponent<AbletoTeleport>();
        Debug.Log($"Object entered trigger: {collider.gameObject.name}");

        if (abletotelport != null)
        {
            Debug.Log($"AbletoTeleport component found on: {collider.gameObject.name}");
            OnEnter(abletotelport);
        }
        else
        {
            Debug.LogWarning($"Object {collider.gameObject.name} does not have an AbletoTeleport component!");
        }
    }

    public void OnEnter(AbletoTeleport teleportable)
    {
        Debug.Log($"Attempting to teleport object: {teleportable.gameObject.name}");
        if (!teleportable.TeleportAble)
        {
            Debug.LogWarning("Teleportation denied. Object is not teleportable.");
            return;
        }

        teleportable.TeleportAble = false;

        if (SceneManager.GetActiveScene().name == destinationSceneName)
        {
            Debug.Log("Teleporting within the same scene.");
            Teleport(teleportable);
        }
        else
        {
            Debug.Log($"Teleporting to new scene: {destinationSceneName}");
            StartCoroutine(TeleportToNewScene(destinationSceneName, teleportable));
        }
    }

    private IEnumerator TeleportToNewScene(string sceneName, AbletoTeleport teleportable)
    {
        Debug.Log($"Loading scene: {sceneName}");
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation newSceneAsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!newSceneAsyncLoad.isDone)
        {
            Debug.Log("Scene loading...");
            yield return null;
        }

        Debug.Log($"Scene {sceneName} loaded. Moving object to new scene.");
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.MoveGameObjectToScene(teleportable.gameObject, newScene);
        SceneManager.SetActiveScene(newScene); // Ensure new scene is active

        yield return new WaitForSeconds(0.1f); // Small delay to allow scene to initialize

        Teleport(teleportable);

        Debug.Log($"Unloading current scene: {currentScene.name}");
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void Teleport(AbletoTeleport teleportable)
    {
        SpawningPoint spawnPoint = FindSpawnPoint(destSpawnName);
        if (spawnPoint != null)
        {
            Debug.Log($"Teleporting {teleportable.gameObject.name} to spawn point: {spawnPoint.spawnName}");
            teleportable.transform.position = spawnPoint.transform.position;
            teleportable.TeleportAble = true; // Reset after successful teleport
        }
        else
        {
            Debug.LogWarning("No valid spawn point found. Teleportation aborted.");
            teleportable.TeleportAble = true; // Reset even if teleportation fails
        }
    }

    private SpawningPoint FindSpawnPoint(string spawnName)
    {
        Debug.Log($"Searching for spawn point: {spawnName}");

        
        SpawningPoint[] spawnPoints = FindObjectsByType<SpawningPoint>(FindObjectsSortMode.None);

        foreach (SpawningPoint spawn in spawnPoints)
        {
            if (spawn.spawnName == spawnName)
            {
                Debug.Log($"Spawn point found: {spawnName}");
                return spawn;
            }
        }

        Debug.LogWarning($"No spawn point found with name: {spawnName}");
        return null;
    }
}
