using UnityEngine;
using UnityEngine.InputSystem;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] private GameObject trapToSpawn;
    public Transform trapModel;
    [SerializeField] protected float offset;
    [SerializeField] protected float verticalSearch; //max distance for raycast
    [SerializeField] protected LayerMask whatIsBuildable;// detecting groundlayer for raycast 
    [SerializeField] protected LayerMask obstacleMasks;
    [SerializeField] protected int trapPrice;
    public bool isDebugging;
    public Material Hologram;
    public bool CanPlace;

    [SerializeField] private float obstacleDetectionRange = 3;

    Actor_Player player => LevelManager.Instance.Player;

    //Audio Settings
    AudioCue ACue;
    public event System.Action OnActivated;

    public virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    protected void Start()
    {

    }

    private void Update()
    {
        transform.position = LevelManager.Instance.Player.TrapHolder.position + (LevelManager.Instance.Player.transform.forward * offset);

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.position, Vector3.down); //cast from trap to spawn 
        Debug.DrawRay(transform.position, Vector3.down, Color.green);

        Collider[] obstacles = Physics.OverlapBox(trapModel.position, Vector3.one * obstacleDetectionRange, Quaternion.identity, obstacleMasks);

        if (Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable))
        {
            trapModel.position = outHit.transform.position;
            
        }

        CanPlace = obstacles.Length <= 0 && player.Controller.isGrounded && outHit.collider != null;

        if (CanPlace)
        {
            trapModel.position = outHit.transform.position;
            Hologram.SetColor("Color_F3B47044", Color.blue); //assigning placeable trap colour to blue 
        }
        else
        {

            Hologram.SetColor("Color_F3B47044", Color.red);// Changing the colour of the trap to red if it can't be placed
            CanPlace = false;
        }

    }

    public virtual void HandleInput(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Fire":

                if (context.phase == InputActionPhase.Performed)
                    PlaceTrap();

                break;

            case "Alt Fire":

                if (context.phase == InputActionPhase.Performed)
                    RotateTrap();

                break;
        }
    }
    protected void PlaceTrap()
    {
        //Debug.Log("IS THIS BEING CALLED");
        if (LevelManager.Instance.CurrentEnergy < trapPrice)
            return;

        //setting the trap GameObject to spawn on raycast's position IF its on whatIsbuildable
        if (CanPlace)
        {
            GameObject tempTrap = Instantiate(trapToSpawn, trapModel.position, transform.rotation); //instantiating trap 
            //ACue.PlayAudioCue();
            LevelManager.Instance.CurrentEnergy -= trapPrice;
            LevelManager.Instance.GetComponent<AudioCue>().PlayAudioCue(LevelManager.Instance.Char_SO.BuildTrap, 20);
        }
    }

    public void RotateTrap()
    {
        //Debug.Log("Rotating trap");
        //transform.Rotate(transform.rotation.eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles); //rotating trap by 90 degrees clockwise 
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
    }

    public void Equip()
    {
        //resetting gameObject's position then push it a bit forward
        transform.SetParent(LevelManager.Instance.Player.TrapHolder);
        transform.localPosition = Vector3.zero;
        transform.localPosition += transform.forward * offset;
        transform.SetParent(null);
        gameObject.SetActive(true); //setting trap to activate when equiping 

        // Registering inputs when we equip this.
        LevelManager.Instance.Player.playerInputs.onActionTriggered += HandleInput;
    }

    public void Unequip()
    {
        gameObject.SetActive(false); //setting trap to deActivate when unEquipping 

        // Deregistering inputs when we unequip this.
        LevelManager.Instance.Player.playerInputs.onActionTriggered -= HandleInput;
    }

    protected void OnDrawGizmos()
    {
        if(isDebugging)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - verticalSearch, transform.position.z), Color.green);
        }

        Gizmos.DrawWireCube(trapModel.position, Vector3.one * obstacleDetectionRange);
    }
}
