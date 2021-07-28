using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyInput
{
  public string key;
  public bool buffer;
  float activetime;
  public float buffertime;
  string buttondown = "n";

  public bool Down()
  {
    if(buffer)
    {
      bool active = Active();
      if(Input.GetKeyDown(key) && buttondown == "n")
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return(Input.GetKeyDown(key));
    }
  }

  public bool Active()
  {
    if(buffer)
    {
      if(Input.GetKey(key))
      {
        activetime = buffertime;
        buttondown = "y";
        return(true);
      }
      else
      {
        activetime -= Time.deltaTime;
        if(activetime >= 0f)
        {
          buttondown = "y";
          return true;
        }
        else
        {
          if(buttondown == "y")
          {
            buttondown = "up";
          }
          return false;
        }
      }
    }
    else
    {
      return(Input.GetKey(key));
    }
  }

  public bool Up()
  {
    if(buffer)
    {
      bool down = Down();
      if(buttondown == "up")
      {
        buttondown = "n";
        return true;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return(Input.GetKeyUp(key));
    }
  }
}
