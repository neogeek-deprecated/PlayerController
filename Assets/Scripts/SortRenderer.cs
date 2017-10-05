using UnityEngine;

public class SortRenderer : MonoBehaviour {

    public string sortingLayerName;
    public new Renderer renderer;

    void Awake() {

        renderer.sortingLayerName = sortingLayerName;

    }

}
