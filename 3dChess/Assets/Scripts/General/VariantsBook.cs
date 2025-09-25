using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VariantsBook : MonoBehaviour
{
    public BookSectionUI OriginalBookSection;
    public ContentSizeFitter Fitter;

    private void Start()
    {
        Generate();
    }
    public void Generate()
    {
        foreach (var variant in Variants.PiecesVariants.Select(p => p.Variant).Distinct())
        {
            var newSection = Instantiate(OriginalBookSection, OriginalBookSection.transform.parent);
            newSection.gameObject.SetActive(true);
            newSection.Create(variant);
        }
        Fitter.enabled = false;
        Invoke(nameof(ReEnableFitter), 0.5f);
    }

    public void ReEnableFitter() => Fitter.enabled = true;
}
