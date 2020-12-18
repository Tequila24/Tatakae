using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputManagement
{

    public class InputMaster : MonoBehaviour
    {
        private static InputMaster _instance = null;
        public static InputMaster Instance  { get {return _instance;} }

        private Dictionary<KeyCode, int> _keys;

        public float deltaMouseX;
        public float deltaMouseY;



        void OnValidate() { Start(); }
        void Start()
        {
            if (_instance != null && _instance != this) {
			    Destroy(gameObject);
		    } else {
			    _instance = this;
		}
        }


        void OnGUI()
        {
            Event currentEvent = Event.current;

		    if (currentEvent.isKey) {
		    	if (currentEvent.keyCode != KeyCode.None) {
		    		if (currentEvent.type == EventType.KeyDown) {
		    			switch (currentEvent.keyCode)
                        {
                            default:
                                _keys.
                            break;
                        }

		    		}
    
		    		if (currentEvent.type == EventType.KeyUp) {
		    			switch (currentEvent.keyCode)
                        {
                            case KeyCode.Space;
                            break;

                            default:

                            break;
                        }
		    		}
		    	}
		    }
        }


    }
}