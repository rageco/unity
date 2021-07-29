using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GI
{
  public static KeyInput[] keys;
  public static bool buffermode;
  public static float buffertime;

  public static void SetBuffer(bool activate)
  {
    foreach(KeyInput key in keys)
    {
      key.buffer = activate;
      key.buffertime = buffertime;
    }
    buffermode = activate;
  }

  public static bool KeyDown(string keycode)
  {
    KeyInput key = Array.Find(keys, key=> key.use == keycode);
    if(key != null)
    {
      return(key.Down());
    }
    else
    {
      return(Input.GetKeyDown(keycode));
    }
  }

  public static bool Key(string keycode)
  {
    KeyInput key = Array.Find(keys, key=> key.use == keycode);
    if(key != null)
    {
      return(key.Active());
    }
    else
    {
      return(Input.GetKey(keycode));
    }
  }

  public static bool KeyUp(string keycode)
  {
    KeyInput key = Array.Find(keys, key=> key.use == keycode);
    if(key != null)
    {
      return(key.Up());
    }
    else
    {
      return(Input.GetKeyUp(keycode));
    }
  }
}
