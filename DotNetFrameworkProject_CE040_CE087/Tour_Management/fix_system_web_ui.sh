#!/bin/bash
# Fix all System.Web.UI references by removing them from using statements
# and keeping the fully qualified types

for file in *.cs *.aspx.designer.cs; do
    if [ -f "$file" ]; then
        # Remove using System.Web.UI lines
        sed -i '/^using System\.Web\.UI;$/d' "$file" 2>/dev/null || true
        sed -i '/^using System\.Web\.UI\.WebControls;$/d' "$file" 2>/dev/null || true
        sed -i '/^using System\.Web\.UI\.HtmlControls;$/d' "$file" 2>/dev/null || true
    fi
done
echo "Removed System.Web.UI using statements"
