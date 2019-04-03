"""Blender Plug-in for Nif import and export."""

# ***** BEGIN LICENSE BLOCK *****
#
# Copyright © 2005-2015, NIF File Format Library and Tools contributors.
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions
# are met:
#
#    * Redistributions of source code must retain the above copyright
#      notice, this list of conditions and the following disclaimer.
#
#    * Redistributions in binary form must reproduce the above
#      copyright notice, this list of conditions and the following
#      disclaimer in the documentation and/or other materials provided
#      with the distribution.
#
#    * Neither the name of the NIF File Format Library and Tools
#      project nor the names of its contributors may be used to endorse
#      or promote products derived from this software without specific
#      prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
# "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
# LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
# FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
# COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
# INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
# BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
# CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
# LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
# ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
# POSSIBILITY OF SUCH DAMAGE.
#
# ***** END LICENSE BLOCK *****

import logging
import os
import sys

import bpy
import bpy.props

# Python dependencies are bundled inside the io_scene_mnif/dependencies folder
_dependencies_path = os.path.join(os.path.dirname(__file__), "dependencies")
if _dependencies_path not in sys.path:
    sys.path.append(_dependencies_path)
    print(sys.path)
del _dependencies_path

import io_scene_mnif
from io_scene_mnif import properties, operators, ui


try:
    from io_scene_mnif.utility import nif_debug
    nif_debug.start_debug()
except:
    print("Failed to load debug module")

# Blender addon info.
bl_info = {
    "name": "[M] NetImmerse/Gamebryo nif format",
    "description": "[M] Import and export files in the NetImmerse/Gamebryo nif format (.nif)",
    "author": "Markellus, based on NifTools by NifTools Team",
    "blender": (2, 7, 9),
    "version": (1, 0, 0),
    "api": 39257,
    "location": "File > Import-Export",
    "warning": "Alpha",
    "wiki_url": "",
    "tracker_url": "",
    "support": "COMMUNITY",
    "category": "Import-Export"}


def _init_loggers():
    """Set up loggers."""
    niftools_logger = logging.getLogger("niftools")
    niftools_logger.setLevel(logging.WARNING)
    pyffi_logger = logging.getLogger("pyffi")
    pyffi_logger.setLevel(logging.WARNING)
    log_handler = logging.StreamHandler()
    log_handler.setLevel(logging.DEBUG)
    log_formatter = logging.Formatter("%(name)s:%(levelname)s:%(message)s")
    log_handler.setFormatter(log_formatter)
    niftools_logger.addHandler(log_handler)
    pyffi_logger.addHandler(log_handler)


# noinspection PyUnusedLocal
def menu_func_import(self, context):
    self.layout.operator(operators.nif_import_op.NifImportOperator.bl_idname, text="NetImmerse/Gamebryo (.nif)")
    self.layout.operator(operators.kf_import_op.KfImportOperator.bl_idname, text="NetImmerse/Gamebryo (.kf)")
    # TODO: get default path from config registry
    # default_path = bpy.data.filename.replace(".blend", ".nif")
    # ).filepath = default_path


# noinspection PyUnusedLocal
def menu_func_export(self, context):
    self.layout.operator(operators.nif_export_op.NifExportOperator.bl_idname, text="NetImmerse/Gamebryo (.nif)")
    # self.layout.operator(operators.kf_export_op.KfExportOperator.bl_idname, text="NetImmerse/Gamebryo (.kf)")


def register():
    _init_loggers()
    properties.register()
    ui.register()
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_import.append(menu_func_import)
    bpy.types.INFO_MT_file_export.append(menu_func_export)


def unregister():
    # no idea how to do this... oh well, let's not lose any sleep over it uninit_loggers()
    bpy.types.INFO_MT_file_import.remove(menu_func_import)
    bpy.types.INFO_MT_file_export.remove(menu_func_export)
    bpy.utils.unregister_module(__name__)


if __name__ == "__main__":
    register()