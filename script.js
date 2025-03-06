console.log("✅ script.js has been loaded successfully!");

document.addEventListener("DOMContentLoaded", function () {
    function notifyUnity() {
        console.log("📤 Sending message to Unity: HideRecycleBin");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("Systems", "ReceiveMessage", "HideRecycleBin");
            console.log("✅ Successfully sent message to Unity!");
        }
    }

    //----------------------------------- Change Images (Light to Dark) -----------------------------------------------

    function changeTableImage() {
        const imageContainer = document.getElementById("table-background"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Table Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

    function changeOverlayImage() {
        const imageContainer = document.getElementById("overlay-image"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Monitor Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

    function changePostItImage() {
        const imageContainer = document.getElementById("postit-note-1"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Layer 2 Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

    //--------------------------------------------------------------------------------------------------

    // ✅ Add click event listener to button
    const button = document.getElementById("hideRecycleBinButton");
    if (button) {
        button.addEventListener("click", function () {
            console.log("🖱️ Button clicked! Sending message to Unity.");
            notifyUnity();
        });
    }

    // ✅ Monitor comment changes
    console.log("✅ Starting MutationObserver for comment tracking...");
    let commentNode = null;
    document.body.childNodes.forEach((node) => {
        if (node.nodeType === Node.COMMENT_NODE && node.nodeValue.includes("ChatAPT Permission Level")) {
            commentNode = node;
        }
    });

    if (commentNode) {
        const observer = new MutationObserver(() => {
            console.log(`📝 Comment changed: ${commentNode.nodeValue}`);
            //notifyUnity();
            changeTableImage();
            changeOverlayImage();
            changePostItImage();
        });
        observer.observe(commentNode, { characterData: true, subtree: true });

        // ✅ Ensure updates even if tab is inactive
        setInterval(() => {
            if (document.hidden) {
                observer.takeRecords();
            }
        }, 500);
    } else {
        console.error("❌ Comment node not found!");
    }

    // ✅ Monitor background color changes
    console.log("✅ Starting MutationObserver for background color...");
    const targetNode = document.getElementById("background-wallpaper");

    if (!targetNode) {
        console.error("❌ Target element #background-wallpaper not found!");
        return;
    }

    console.log("✅ Target Node found successfully!");
    let lastColor = window.getComputedStyle(targetNode).backgroundColor;

    // ✅ MutationObserver for inline style changes
    const bgObserver = new MutationObserver(() => {
        let newColor = window.getComputedStyle(targetNode).backgroundColor;
        if (newColor !== lastColor) {
            console.log(`🎨 Background color changed! New color: ${newColor}`);
            lastColor = newColor;
            notifyUnity();
        }
    });

    bgObserver.observe(targetNode, { attributes: true, attributeFilter: ["style"] });

    console.log("✅ MutationObserver is monitoring background color changes...");

    // ✅ Fallback polling method (detects computed styles, even if changed via CSS)
    setInterval(() => {
        let newColor = window.getComputedStyle(targetNode).backgroundColor;
        if (newColor !== lastColor) {
            console.log(`🎨 Background color changed via computed style! New color: ${newColor}`);
            lastColor = newColor;
            notifyUnity();
        }
    }, 500);
});
