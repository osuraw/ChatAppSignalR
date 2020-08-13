var createroombutton = document.getElementById("create-room-btn")
var createroommodal = document.getElementById("create-room-modal")

createroombutton.addEventListener('click',function name() {
    createroommodal.classList.add('active')
})

closemodal = function () {
    createroommodal.classList.remove('active')    
}