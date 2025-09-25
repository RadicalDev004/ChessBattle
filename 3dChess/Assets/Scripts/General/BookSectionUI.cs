using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BookSectionUI : MonoBehaviour
{
    public BookPieceUI OriginalPiece;
    public string VariantName;
    public TMP_Text T_Name;

    public void Create(string variant)
    {
        VariantName = variant;
        T_Name.text = VariantName.ToUpper();

        foreach (var p in Variants.PiecesVariants.Where(p => p.Variant == variant))
        {
            var newPiece = Instantiate(OriginalPiece, OriginalPiece.transform.parent);
            newPiece.gameObject.SetActive(true);
            newPiece.Create(p);
        }
    }
}
