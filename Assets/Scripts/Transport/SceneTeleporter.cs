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
       

        if (abletotelport != null)
        {
            
            OnEnter(abletotelport);
        }
        else
        {
           
        }
    }

    public void OnEnter(AbletoTeleport teleportable)
    {
        
        if (!teleportable.TeleportAble)
        {
           
            return;
        }

        teleportable.TeleportAble = false;

        if (SceneManager.GetActiveScene().name == destinationSceneName)
        {
            
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
       
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation newSceneAsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!newSceneAsyncLoad.isDone)
        {
          
            yield return null;
        }

       
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.MoveGameObjectToScene(teleportable.gameObject, newScene);
        SceneManager.SetActiveScene(newScene); // Ensure new scene is active

        yield return new WaitForSeconds(0.1f); // Small delay to allow scene to initialize

        Teleport(teleportable);

       
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void Teleport(AbletoTeleport teleportable)
    {
        SpawningPoint spawnPoint = FindSpawnPoint(destSpawnName);
        if (spawnPoint != null)
        {
           
            teleportable.transform.position = spawnPoint.transform.position;
            teleportable.TeleportAble = true; // Reset after successful teleport
        }
        else
        {
            
            teleportable.TeleportAble = true; // Reset even if teleportation fails
        }
    }

    private SpawningPoint FindSpawnPoint(string spawnName)
    {
       
        
        SpawningPoint[] spawnPoints = FindObjectsByType<SpawningPoint>(FindObjectsSortMode.None);

        foreach (SpawningPoint spawn in spawnPoints)
        {
            if (spawn.spawnName == spawnName)
            {
               
                return spawn;
            }
        }

        
        return null;
    }
}
