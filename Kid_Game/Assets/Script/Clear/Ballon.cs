using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballon : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.CompareTag("SideBallon"))
        {
            this.gameObject.transform.position += new Vector3(0, Random.Range(7, 15), 0) * Time.deltaTime;
        }

        if(this.gameObject.CompareTag("MainBallon") && this.gameObject.transform.position.y <= 0.0f)
        {
            this.gameObject.transform.position += new Vector3(0, Random.Range(7, 15), 0) * Time.deltaTime;
        }

        if(this.gameObject.transform.position.y > 12)
        {
            Destroy(this.gameObject);
        }
    }
}
