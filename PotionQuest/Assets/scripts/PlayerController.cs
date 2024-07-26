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

    public float lookSpeed = 1.5f;
    private float lookXLimit = 85f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    public PlayerInventory inventory;
    private Item selectedItem;
    public Transform ItemDisplay;
    protected GameObject ItemDisplayObject;

    public TextMeshProUGUI textMeshPro;
    public LightManager lightManager;

    private AudioSource footstepAudio;
    Vector3 lastFootstepPosition;
    public float FootStepDistance = 6;

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
    ///////////////////////////////

    void Update()
    {

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
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        bool inLight = false;
        //check to see whether or not we are in shadowwwwww
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
                    if (!Physics.Raycast(transform.position, ToLight.normalized, ToLight.magnitude, 7, QueryTriggerInteraction.Ignore))
                    {
                        //something is blocking the light.
                        inLight = true;
                    }
                }
            }
            else
            {
                //directional or other
                //just raycast at the light direction and see if it hits something
                Vector3 LightDirection = light.gameObject.transform.forward;
                if (!Physics.Raycast(transform.position, -LightDirection, 50, 7, QueryTriggerInteraction.Ignore))
                {
                    inLight = true;
                }
            }
        }
        if (inLight)
        {
            // Debug.Log("We are in light!");
        }

        //display your selected item in your hands
        if (selectedItem != inventory.items[inventory.selectedSlot].item)
        {
            selectedItem = inventory.items[inventory.selectedSlot].item;
            //update display
            if (selectedItem != null)
            {
                Destroy(ItemDisplayObject);

                ItemDisplayObject = Instantiate(selectedItem.itemObject, ItemDisplay.transform.position + selectedItem.offsetPosition, ItemDisplay.transform.rotation);
                ItemDisplayObject.transform.rotation = Quaternion.Euler(selectedItem.offsetRotation);
                ItemDisplayObject.transform.localScale *= selectedItem.offsetScale;
                ItemDisplayObject.transform.parent = ItemDisplay;
                if (ItemDisplayObject.TryGetComponent<ItemPickup>(out ItemPickup comp))
                {
                    comp.CanBePickedUp = false;
                }
            }
            else
            {
                Destroy(ItemDisplayObject);
            }


        }

        //interact with things in front of us
        RaycastHit[] results;
        results = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 5);
        textMeshPro.text = "";
        foreach (RaycastHit hit in results)
        {
            //show what text needs to be shown to help with interaction
            //interact with interactables!
            if (hit.transform.gameObject.TryGetComponent<Interactable>(out Interactable obj))
            {
                textMeshPro.text = obj.GetHoveredText();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    obj.Press();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    obj.LeftClick();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    obj.RightClick();
                }
                break;
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
                            int amountAdded = display.inventory.AddItemToSlot(display.slotIndex, inventory.items[inventory.selectedSlot].item, inventory.items[inventory.selectedSlot].stackSize);
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
                        int amountAdded = inventory.AddItemToSlot(inventory.selectedSlot, stack.item, stack.stackSize);
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

                break;
            }
        }
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
}
