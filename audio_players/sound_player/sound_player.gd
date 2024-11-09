class_name SoundPlayer
extends Node


## Add this scene as a child and, in the parent, call like such:
## $SoundPlayer/Splosh.play()
## $SoundPlayer/Woosh.play()


func play(sound_name:String):
	
	find_child(sound_name).play()
