using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]

// CITE: Code from pastebin found in YouTube Video
// LINK: https://www.youtube.com/watch?v=qQLvcS9FxnY
public class PlayerController : MonoBehaviour
{

    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 50f;

    public bool inLight = false;

    public float lookSpeed = 1.5f;
    private float lookXLimit = 85f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    public PlayerInventory inventory;
    private Item selectedItem;
    public GameObject Hands;
    public Transform ItemDisplay;
    protected GameObject ItemDisplayObject;

    public TextMeshProUGUI textMeshPro;
    public LightManager lightManager;

    //footsteps
    public AudioSource footstepAudio;
    Vector3 lastFootstepPosition;
    public float FootStepDistance = 6;

    //pickup sound
    public AudioSource pickupAudio;

    public bool Respawn = false;
    private bool Teleported = false;
    public float WaterHeight = 48f;
    public Vector3 WellPosition;
    
    private bool HoldingStarlight = false;
    public float StarlightPotionDuration = 400.0f;
    public Item EmptyStarlight;
    void Start()
    {
        inventory.OnInventoryChanged += UpdateMass;
        characterController = GetComponent<CharacterController>();
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        LockCursor();
        //footstep audio
        footstepAudio = GetComponent<AudioSource>();
        lastFootstepPosition = transform.position;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    ////////adding jump check//////
    public bool IsJumping()
    {
        // This method checks if the player is currently jumping
        return !characterController.isGrounded;
    }

    public void Teleport(Vector3 location)
    {
        characterController.enabled = false;
        Respawn = false;
        transform.position = location;
        Teleported = true;
    }
    ///////////////////////////////

    void Update()
    {
        //remove fluid from starlight. slowly.
        if (HoldingStarlight)
        {
            inventory.items[inventory.selectedSlot].Value -= (1.0f / StarlightPotionDuration) * Time.deltaTime;
            StarlightPotion potion = ItemDisplayObject.GetComponent<StarlightPotion>();
            potion.SetAmount(inventory.items[inventory.selectedSlot].Value);
            if (inventory.items[inventory.selectedSlot].Value <= 0)
            {
                //potion ran out
                inventory.RemoveItemFromSlot(inventory.selectedSlot, 1);
                inventory.AddItemToSlot(inventory.selectedSlot, EmptyStarlight, 1);
            }
        }
        //respawn if you touch the water
        if (transform.position.y <= WaterHeight)
        {
            Respawn = true;
        }

        //if respawn, so the player can be launched at the start of the game and other things can cause a respawn
        if ( Respawn)
        {
            characterController.enabled = false;
            Respawn = false;
            transform.position = WellPosition;
            GetComponent<Rigidbody>().velocity = new Vector3(0, 40, 30);
            moveDirection.z = 10;
            moveDirection.y = 40;
            return;
        }else
        {
            characterController.enabled = true;
        }
        if (Teleported)
        {
            Teleported = false;
            return;
        }

        // Movement Logic
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        
        //getting rid of diagonal fix cause idk what the canmove thing is doin and haven't looked at rest of code lol

        //footsteps
        if (characterController.isGrounded && (lastFootstepPosition - transform.position).sqrMagnitude > FootStepDistance)
        {
            lastFootstepPosition = transform.position;
            footstepAudio.Play();
        }

        // Jump Logic
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Camera Logic
        //if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        
        //check to see whether or not we are in shadowwwwww
        //assume we are
        inLight = GetLightStatus();
        
        //see if we want to drop held item
        if (Input.GetKeyDown(KeyCode.G))
        {
            //drop held item
            if (inventory.items[inventory.selectedSlot] != null)
            {
                GameObject obj = Instantiate(inventory.items[inventory.selectedSlot].item.itemObject, this.transform.position + new Vector3(0, -0.9f, 0), this.transform.rotation);
                obj.transform.Rotate(inventory.items[inventory.selectedSlot].item.offsetRotation);
                if (obj.TryGetComponent<Potion>(out Potion pot))
                {
                    pot.SetAmount(inventory.items[inventory.selectedSlot].Value);
                    pot.onGround = true;
                }
                //remove the item from inv now that it is dropped
                inventory.RemoveItemFromSlot(inventory.selectedSlot, 1);
            }
        }

        //display your selected item in your hands
        if (selectedItem != inventory.items[inventory.selectedSlot].item)
        {
            selectedItem = inventory.items[inventory.selectedSlot].item;
            //update display
            if (selectedItem != null)
            {
                Destroy(ItemDisplayObject);
                HoldingStarlight = false;

                Hands.SetActive(true);

                ItemDisplayObject = Instantiate(selectedItem.itemObject, ItemDisplay.transform.position, ItemDisplay.transform.rotation);
                //ItemDisplayObject.transform.rotation = Quaternion.Euler(ItemDisplay.transform.rotation.eulerAngles);
                ItemDisplayObject.transform.Rotate(selectedItem.offsetRotation);
                ItemDisplayObject.transform.localScale *= selectedItem.offsetScale;
                ItemDisplayObject.transform.parent = ItemDisplay;
                ItemDisplayObject.transform.localPosition += selectedItem.offsetPosition;
                if (ItemDisplayObject.TryGetComponent<Collider>(out Collider collider))
                {
                    collider.enabled = false;
                }
                if (ItemDisplayObject.TryGetComponent<Potion>(out Potion potion))
                {
                    //we are displaying a potion. we need to render how much of the potion is left and tell the script the amount
                    potion.SetAmount(inventory.items[inventory.selectedSlot].Value);
                    if (inventory.items[inventory.selectedSlot].item.itemName == "StarlightPotion")
                    {
                        HoldingStarlight = true;
                    }
                }
                // if (ItemDisplayObject.TryGetComponent<ItemPickup>(out ItemPickup comp))
                // {
                //     comp.CanBePickedUp = false;
                // }
            }
            else
            {
                Destroy(ItemDisplayObject);
                HoldingStarlight = false;
                Hands.SetActive(false);
            }


        }

        //interact with things in front of us
        //RaycastHit[] results;
        //results = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 5);
        Physics.Raycast(playerCamera.transform.position,playerCamera.transform.forward, out RaycastHit hit, 5);
        textMeshPro.text = "";
        //foreach (RaycastHit hit in results)
        if (hit.transform != null){
            //show what text needs to be shown to help with interaction
            //interact with interactables!
            if (hit.transform.gameObject.tag == "Interactable")
            {
                //Debug.Log("hit interactable");
                GameObject obj = hit.transform.gameObject;
                while (obj.transform.parent != null && !hit.transform.gameObject.TryGetComponent<Interactable>(out Interactable i) && obj.tag == "Interactable")
                {
                    obj = obj.transform.parent.gameObject;
                    //Debug.Log("moving up a parent");
                }
                if (obj.transform.gameObject.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    textMeshPro.text = interactable.GetHoveredText();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Press();
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        interactable.LeftClick();
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        interactable.RightClick();
                    }
                    //break;
                }
            }
            

            if (hit.transform.gameObject.TryGetComponent<InventorySlotDisplay>(out InventorySlotDisplay display))
            {
            
                if (display.inventory.items[display.slotIndex].item == null)
                {
                    //don't be able to add items to it if you aren't supposed to
                    if (!display.BlockPlacement)
                    {
                        //see if we just pressed E, and if we did then place 
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            int amountAdded = display.inventory.AddItemToSlot(display.slotIndex, inventory.items[inventory.selectedSlot].item, inventory.items[inventory.selectedSlot].stackSize, inventory.items[inventory.selectedSlot].Value);
                            inventory.RemoveItemFromSlot(inventory.selectedSlot, amountAdded);
                        }
                        if (inventory.items[inventory.selectedSlot].item != null)
                            textMeshPro.text = "Place Item";
                    }
                }
                else
                {
                    textMeshPro.text = "Pick Up Item";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        //there is already an item there, so try to take the item
                        ItemStack stack = display.inventory.items[display.slotIndex];
                        int amountAdded = inventory.AddItemToSlot(inventory.selectedSlot, stack.item, stack.stackSize, stack.Value);
                        if (amountAdded != 0)
                        {
                            display.inventory.RemoveItemFromSlot(display.slotIndex, amountAdded);
                            if (display.inventory.items[display.slotIndex].stackSize < 1)
                            {
                                display.DisplayItem(null);
                            }
                        }
                        
                    }

                }

                //break;
            }
        }
    }

    public void PlayPickupSound()
    {
        pickupAudio.Play();
    }

    private void UpdateMass()
    {
        float newMass = 1;
        foreach (ItemStack stack in inventory.items)
        {
            if (stack.item != null)
                newMass += stack.item.itemWeight * stack.stackSize;
        }
        GetComponent<Rigidbody>().mass = newMass;
    }

    public bool GetLightStatus()
    {
        bool lit = false;
        foreach (SafeLight light in lightManager.lights)
        {
            if (light.type == LightType.PointLight)
            {
                //point light
                //get the range of the point light, and exlude it if we are past that
                Vector3 ToLight = light.gameObject.transform.position - transform.position;

                if (ToLight.sqrMagnitude <= light.light.range * light.light.range)
                {
                    //the distance to the light is lesser than the lights range, so lets scan for anything blocking the light
                    if (!Physics.Raycast(transform.position, ToLight.normalized, ToLight.magnitude, 8, QueryTriggerInteraction.Ignore))
                    {
                        //something is blocking the light.
                        lit = true;
                        break;
                    }
                }
            }
            else
            {
                //we are going to ignore directional light for now
                
                // //directional or other
                // //just raycast at the light direction and see if it hits something
                // Vector3 LightDirection = light.gameObject.transform.forward;
                // //Debug.DrawRay(transform.position, -LightDirection, Color.yellow, 1);
                // if (!Physics.Raycast(transform.position - LightDirection, -LightDirection, 300))
                // {
                //     Debug.Log("lit from sun");
                //     lit = true;
                //     break;
                // }
            }
        }
        return lit;
    }
}
