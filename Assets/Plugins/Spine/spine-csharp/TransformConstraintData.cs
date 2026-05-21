/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;

namespace Spine
{
    public class TransformConstraintData
    {
        internal string name;
        internal int order;
        internal ExposedList<BoneData> bones = new();
        internal BoneData target;
        internal float rotateMix, translateMix, scaleMix, shearMix;
        internal float offsetRotation, offsetX, offsetY, offsetScaleX, offsetScaleY, offsetShearY;
        internal bool relative, local;

        public string Name { get => name; }

        public int Order { get => order; set => order = value; }

        public ExposedList<BoneData> Bones { get => bones; }

        public BoneData Target { get => target; set => target = value; }

        public float RotateMix { get => rotateMix; set => rotateMix = value; }

        public float TranslateMix { get => translateMix; set => translateMix = value; }

        public float ScaleMix { get => scaleMix; set => scaleMix = value; }

        public float ShearMix { get => shearMix; set => shearMix = value; }

        public float OffsetRotation { get => offsetRotation; set => offsetRotation = value; }

        public float OffsetX { get => offsetX; set => offsetX = value; }

        public float OffsetY { get => offsetY; set => offsetY = value; }

        public float OffsetScaleX { get => offsetScaleX; set => offsetScaleX = value; }

        public float OffsetScaleY { get => offsetScaleY; set => offsetScaleY = value; }

        public float OffsetShearY { get => offsetShearY; set => offsetShearY = value; }

        public bool Relative { get => relative; set => relative = value; }

        public bool Local { get => local; set => local = value; }

        public TransformConstraintData(string name)
        {
            if (name == null) throw new ArgumentNullException("name", "name cannot be null.");
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}