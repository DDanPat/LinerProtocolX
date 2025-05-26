using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeColor : MonoBehaviour
{
    [System.Serializable]
    public class MaterialSlot
    {
        public MeshRenderer targetRenderer;
        public int materialIndex;
    }
    
    [SerializeField] private List<Material> gradeMaterials = new List<Material>();
    
    public List<MaterialSlot> targetMaterials;


    public void SetGradeMaterial(int grade)
    {
        foreach (var materialSlot in targetMaterials)
        {
            var renderer = materialSlot.targetRenderer;
            var materials = renderer.materials; // 복사본

            if (materialSlot.materialIndex < 0 || materialSlot.materialIndex >= materials.Length)
            {
                Debug.LogWarning("잘못된 머티리얼 인덱스: " + materialSlot.materialIndex);
                continue;
            }

            materials[materialSlot.materialIndex] = gradeMaterials[grade];

            renderer.materials = materials;
        }
    }
}
