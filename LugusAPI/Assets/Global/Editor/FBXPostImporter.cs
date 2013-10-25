using UnityEditor;
using UnityEngine;

class FBXPostprocessor : AssetPostprocessor
{
    // This method is called just before importing an FBX.
    void OnPreprocessModel()
    {
        ModelImporter mi = (ModelImporter)assetImporter;
        mi.globalScale = 1;

        // Materials for characters are created using the GenerateMaterials script.
        //mi.generateMaterials = 0;
		mi.importMaterials = false;
		
		mi.generateSecondaryUV = true;
		
		Debug.Log ("Model post processed " + mi.assetPath);
    }

}
