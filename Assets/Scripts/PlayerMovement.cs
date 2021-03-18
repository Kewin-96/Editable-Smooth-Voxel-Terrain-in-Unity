using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Controller reference
    public CharacterController controller;

    // Physics parameters
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector3 velocity;

    // Ground checking (for jumping)
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    // Shooting
    public Camera fpsCam;
    public float range = 100000f;
    public int weaponIndex = 0;
    public readonly string[] weaponNames2 = { "One voxel -202", "WEAPON NOT CREATED", "WEAPON NOT CREATED", "WEAPON NOT CREATED", "WEAPON NOT CREATED" };

    /// <summary>
    /// Shooting
    /// </summary>
    public void Shoot()
    {
        if (weaponIndex == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                ChunkSVT hitChunk = hit.transform.GetComponent<ChunkSVT>();
                if (hitChunk != null)
                {
                    EditSVT.DecreaseVoxel(-20, hitChunk, hit.point);
                }
            }
        }
        else if (weaponIndex == 1)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update information if character is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Set velocity to minimum when character is on the ground
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Moving
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Checking height - if below world - teleport to point x,10,z
        if (transform.position.y < -10)
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);

        // Shooting
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }

        // Changing weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIndex = 0;
            Debug.Log("Chosen weapon: " + weaponNames2[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponIndex = 1;
            Debug.Log("Chosen weapon: " + weaponNames2[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponIndex = 2;
            Debug.Log("Chosen weapon: " + weaponNames2[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weaponIndex = 3;
            Debug.Log("Chosen weapon: " + weaponNames2[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weaponIndex = 4;
            Debug.Log("Chosen weapon: " + weaponNames2[4]);
        }
    }
}
