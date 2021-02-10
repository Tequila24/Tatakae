using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController
{
    public Texture2D cs_orange;
    public Texture2D cs_blue;

    public CrosshairController()
    {
        cs_orange = Resources.Load<Texture2D>("textures/cs_orange");
        cs_blue = Resources.Load<Texture2D>("textures/cs_blue");
    }
}
