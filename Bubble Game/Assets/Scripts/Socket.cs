using System;
using UnityEngine;

public class Socket : MonoBehaviour, IInteractableTrigger
{
    public event Action OnActivate;
    public event Action OnDeactivate;

    [SerializeField] private Transform attachmentPoint;
    [SerializeField] private Renderer lightRenderer;

    [SerializeField] private Color offColor;
    [SerializeField] private Color onColor;

    private Transform _item;
    private Material _material;

    void Start()
    {
        Material mat = lightRenderer.material;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", offColor);
        lightRenderer.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        if (_item && _item.parent != attachmentPoint)
        {
            _item = null;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", offColor);
            lightRenderer.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

            OnDeactivate?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PuzzleObject")) return;

        if (other.transform.parent != null) return;

        other.transform.parent = attachmentPoint;
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        _item = other.transform;

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", onColor);
        lightRenderer.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

        OnActivate?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("PuzzleObject")) return;

        if (other.transform.parent != null) return;

        other.transform.parent = attachmentPoint;
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        _item = other.transform;

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", onColor);
        lightRenderer.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

        OnActivate?.Invoke();
    }
}