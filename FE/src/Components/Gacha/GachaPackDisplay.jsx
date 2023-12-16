import './../../styles/GachaPackDisplay.css'

function GachaPackDisplay({Pack, isOpen, onClose}) {

    if (!isOpen) return null;

    return(
        <>
            <div className="Gacha-pack-display" onClick={onClose}>
                <div className="Gacha-pack-display-wrapper" onClick={(event) => {event.stopPropagation()}}>
                    <div className="Gacha-pack-display-img" style={{backgroundImage: `url(${Pack.packimg})`}}></div>
                </div>
            </div>
        </>
    )
}

export default GachaPackDisplay