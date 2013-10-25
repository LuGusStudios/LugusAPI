// Add menu to the main menu
@MenuItem ("GameObject/Create Empty As Child")
static function CreateGameObjectAsChild () {
	var go : GameObject = new GameObject ("GameObject");
	go.transform.parent = Selection.activeTransform;
	go.transform.localPosition = Vector3.zero;
}
 
// The item will be disabled if no transform is selected.
@MenuItem ("GameObject/Create Empty As Child", true)
static function ValidateCreateGameObjectAsChild () {
	return Selection.activeTransform != null;
}
 
// Add context menu to Transform's context menu
@MenuItem ("CONTEXT/Transform/Create Empty As Child")
static function CreateGameObjectAsChild (command:MenuCommand) {
	var tr : Transform = command.context;
	var go : GameObject = new GameObject ("GameObject");
	go.transform.parent = tr;
	go.transform.localPosition = Vector3.zero;
}