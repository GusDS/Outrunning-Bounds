using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    public string nombre;
}

public struct Item2 {
    public string nombre;
}

public class Prueba : MonoBehaviour
{
    Item itempru;
    Item2 itempru2;

    void Start()
    {
        itempru = new Item();
        itempru.nombre = "itempru";
        itempru2.nombre = "itempru2";
        Debug.Log("itempru: " + itempru.nombre);
        Debug.Log("itempru2: " + itempru2.nombre);
        // itempru: itempru
        // itempru2: itempru2

        ModItem(itempru.nombre);
        ModItem(itempru2.nombre);
        Debug.Log("itempru: " + itempru.nombre);
        Debug.Log("itempru2: " + itempru2.nombre);
        // tempString: itempru modificado
        // tempString: itempru2 modificado
        // itempru: itempru                         << pasando string, aunque es reference type se pasa por parámetro como value
        // itempru2: itempru2                       << pasando string, aunque es reference type se pasa por parámetro como value

        ModItem(ref itempru.nombre);
        ModItem(ref itempru2.nombre);
        Debug.Log("itempru: " + itempru.nombre);
        Debug.Log("itempru2: " + itempru2.nombre);
        // tempString: itempru modificado
        // tempString: itempru2 modificado
        // itempru: itempru modificado              << pasando string por ref, ambos se modifican
        // itempru2: itempru2 modificado            << pasando string por ref, ambos se modifican

        itempru.nombre = "itempru";
        itempru2.nombre = "itempru2";
        ModItem(itempru);
        ModItem(itempru2);
        Debug.Log("itempru: " + itempru.nombre);
        Debug.Log("itempru2: " + itempru2.nombre);
        // tempString: itempru modificado
        // tempString: itempru2 modificado
        // itempru: itempru modificado              << pasando class, modifica la referencia
        // itempru2: itempru2                       << pasando struct, el value se modifica dentro de método pero no el value externo
    }

    void ModItem(string tempString)
    {
        tempString += " modificado";
        Debug.Log("tempString: " + tempString);
    }
    void ModItem(ref string tempString)
    {
        tempString += " modificado";
        Debug.Log("tempString: " + tempString);
    }
    void ModItem(Item tempItem)
    {
        tempItem.nombre += " modificado";
        Debug.Log("tempString: " + tempItem.nombre);
    }
    void ModItem(Item2 tempItem)
    {
        tempItem.nombre += " modificado";
        Debug.Log("tempString: " + tempItem.nombre);
    }
}