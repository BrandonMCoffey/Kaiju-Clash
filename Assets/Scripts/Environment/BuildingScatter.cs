using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class DistrictCityGenerator : MonoBehaviour
{
    [Title("General Settings")]
    [SerializeField] private int _seed = 12345;
    [SerializeField, Required] private GameObject _buildingPrefab;
    [SerializeField, ReadOnly] private int _buildingCount;

    [Title("Map Definitions")]
    [SerializeField] private float _citySize = 300f;
    [SerializeField] private int _breakingPointsX = 4;
    [SerializeField] private int _breakingPointsZ = 4;

    [Title("District Rules")]
    [SerializeField] private float[] _allowedStreetSizes = new float[] { 3f, 5f, 8f };
    [SerializeField, MinMaxSlider(5, 40)] private Vector2 _districtBaseSizeRange = new Vector2(10, 25);

    [Title("Height & Variation")]
    [SerializeField] private float _minHeight = 5;
    [SerializeField] private float _maxHeight = 50;
    [Tooltip("Multiplies the calculated height by a random value in this range to create a jagged skyline.")]
    [SerializeField, MinMaxSlider(0.5f, 1.5f)] private Vector2 _heightNoiseRange = new Vector2(0.6f, 1.4f);
    [SerializeField] private AnimationCurve _heightFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [Title("Complex Building Settings")]
    [SerializeField, Range(0, 1)] private float _complexBuildingChance = 0.3f;
    [SerializeField, Range(0, 1)] private float _dimensionMatchChance = 0.5f;
    [InfoBox("Podium height will be between 20% and 40% of the total height.")]
    [SerializeField, MinMaxSlider(0.1f, 0.9f)] private Vector2 _podiumHeightRatio = new Vector2(0.2f, 0.5f);

#if UNITY_EDITOR
    private struct DistrictData
    {
        public Rect Bounds;
        public float BuildingBaseWidth;
        public float BuildingBaseDepth;
        public float StreetSizeX;
        public float StreetSizeZ;
    }

    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    private void GenerateRandom()
    {
        _seed = Random.Range(0, 999999);
        GenerateCity();
    }

    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void GenerateCity()
    {
        ClearCity();
        if (_buildingPrefab == null) return;
        _buildingCount = 0;

        Random.InitState(_seed);

        List<float> cutsX = GenerateCuts(_breakingPointsX, _citySize);
        List<float> cutsZ = GenerateCuts(_breakingPointsZ, _citySize);
        float maxDist = _citySize * 0.6f;

        for (int x = 0; x < cutsX.Count - 1; x++)
        {
            for (int z = 0; z < cutsZ.Count - 1; z++)
            {
                Rect districtRect = Rect.MinMaxRect(cutsX[x], cutsZ[z], cutsX[x + 1], cutsZ[z + 1]);

                DistrictData settings = new DistrictData
                {
                    Bounds = districtRect,
                    BuildingBaseWidth = Random.Range(_districtBaseSizeRange.x, _districtBaseSizeRange.y),
                    BuildingBaseDepth = Random.Range(_districtBaseSizeRange.x, _districtBaseSizeRange.y),
                    StreetSizeX = _allowedStreetSizes[Random.Range(0, _allowedStreetSizes.Length)],
                    StreetSizeZ = _allowedStreetSizes[Random.Range(0, _allowedStreetSizes.Length)]
                };

                FillDistrict(settings, maxDist);
            }
        }
        Debug.Log($"Generated {_buildingCount} buildings");
    }

    [Button(ButtonSizes.Medium), GUIColor(1, 0.5f, 0.5f)]
    private void ClearCity()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void FillDistrict(DistrictData district, float maxDistForHeight)
    {
        float currentZ = district.Bounds.yMin;

        while (currentZ < district.Bounds.yMax)
        {
            float rowTotalDepth = district.BuildingBaseDepth + district.StreetSizeZ;
            float actualRowDepth = Random.Range(district.BuildingBaseDepth * 0.9f, district.BuildingBaseDepth * 1.1f);

            if (currentZ + actualRowDepth > district.Bounds.yMax) break;

            float currentX = district.Bounds.xMin;

            while (currentX < district.Bounds.xMax)
            {
                float colTotalWidth = district.BuildingBaseWidth + district.StreetSizeX;
                float actualColWidth = Random.Range(district.BuildingBaseWidth * 0.9f, district.BuildingBaseWidth * 1.1f);

                if (currentX + actualColWidth > district.Bounds.xMax) break;

                float finalBuildWidth = Random.Range(actualColWidth * 0.5f, actualColWidth);
                float finalBuildDepth = Random.Range(actualRowDepth * 0.5f, actualRowDepth);

                float slackX = actualColWidth - finalBuildWidth;
                float slackZ = actualRowDepth - finalBuildDepth;

                float xPos = currentX + Random.Range(0, slackX);
                float zPos = currentZ + Random.Range(0, slackZ);

                Vector3 spawnPos = new Vector3(xPos + finalBuildWidth / 2, 0, zPos + finalBuildDepth / 2);

                float dist = Vector3.Distance(Vector3.zero, spawnPos);
                float normDist = Mathf.Clamp01(dist / maxDistForHeight);
                float baseHeight = Mathf.Lerp(_minHeight, _maxHeight, _heightFalloff.Evaluate(normDist));

                float finalHeight = baseHeight * Random.Range(_heightNoiseRange.x, _heightNoiseRange.y);

                if (Random.value < _complexBuildingChance)
                {
                    SpawnComplex(spawnPos, finalBuildWidth, finalBuildDepth, finalHeight);
                }
                else
                {
                    SpawnSimple(spawnPos, finalBuildWidth, finalBuildDepth, finalHeight);
                }

                currentX += actualColWidth + district.StreetSizeX;
            }
            currentZ += actualRowDepth + district.StreetSizeZ;
        }
    }

    private void SpawnSimple(Vector3 pos, float w, float d, float h)
    {
        GameObject instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(_buildingPrefab, transform);
        instance.transform.position = pos + Vector3.up * h * 0.5f;
        instance.transform.localScale = new Vector3(w, h, d);
        _buildingCount++;
    }

    private void SpawnComplex(Vector3 pos, float baseW, float baseD, float totalHeight)
    {
        float podiumRatio = Random.Range(_podiumHeightRatio.x, _podiumHeightRatio.y);
        float podiumHeight = totalHeight * podiumRatio;
        float towerHeight = totalHeight - podiumHeight;

        GameObject podium = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(_buildingPrefab, transform);
        podium.transform.position = pos + Vector3.up * podiumHeight * 0.5f;
        podium.transform.localScale = new Vector3(baseW, podiumHeight, baseD);
        _buildingCount++;

        GameObject tower = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(_buildingPrefab, transform);

        float towerW = baseW * Random.Range(0.4f, 0.7f);
        float towerD = baseD * Random.Range(0.4f, 0.7f);

        if (Random.value < _dimensionMatchChance)
        {
            if (Random.value > 0.5f) towerW = baseW;
            else towerD = baseD;
        }

        tower.transform.localScale = new Vector3(towerW, towerHeight, towerD);

        float maxOffsetX = (baseW - towerW) / 2f;
        float maxOffsetZ = (baseD - towerD) / 2f;

        float dirX = (Random.value > 0.5f) ? 1 : -1;
        float dirZ = (Random.value > 0.5f) ? 1 : -1;

        tower.transform.position = pos + new Vector3(maxOffsetX * dirX, podiumHeight + towerHeight * 0.5f, maxOffsetZ * dirZ);
        _buildingCount++;
    }

    private List<float> GenerateCuts(int count, float size)
    {
        List<float> cuts = new List<float>();
        cuts.Add(-size / 2);
        cuts.Add(size / 2);

        for (int i = 0; i < count; i++)
        {
            cuts.Add(Random.Range(-size / 2, size / 2));
        }
        cuts.Sort();
        return cuts;
    }
#endif
}